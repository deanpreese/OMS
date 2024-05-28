import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.metrics import mean_squared_error
from sklearn.preprocessing import MinMaxScaler
from tensorflow.keras.models import Sequential, Model, load_model
from tensorflow.keras.layers import Dense, LeakyReLU, Input, LSTM, concatenate
from tensorflow.keras.optimizers import Adam
import os
import joblib


# Function to create sequences
def create_sequences(data, sequence_length):
    sequences = []
    for i in range(len(data) - sequence_length):
        seq = data[i:i + sequence_length]
        sequences.append(seq)
    return np.array(sequences)

# Function to create the Generator model
def create_generator(input_shape):
    model = Sequential()
    model.add(Input(shape=input_shape))
    model.add(LSTM(100, return_sequences=True))
    model.add(LeakyReLU(alpha=0.2))
    model.add(Dense(input_shape[1]))
    return model

# Function to create the Discriminator model
def create_discriminator(input_shape):
    combined_input = Input(shape=input_shape)
    x = LSTM(100)(combined_input)
    x = LeakyReLU(alpha=0.2)(x)
    x = Dense(50)(x)
    x = LeakyReLU(alpha=0.2)(x)
    output = Dense(1, activation='sigmoid')(x)
    model = Model(combined_input, output)
    return model

# Function to create and compile the GAN model
def create_gan(generators, discriminator, input_shape):
    discriminator.trainable = False
    gan_inputs = [Input(shape=(input_shape[0], input_shape[1])) for _ in generators]
    gen_outputs = [gen(gan_input) for gen, gan_input in zip(generators, gan_inputs)]
    combined_output = concatenate(gen_outputs, axis=-1)  # Combine along the feature dimension
    gan_output = discriminator(combined_output)
    gan = Model(gan_inputs, gan_output)
    gan.compile(loss='binary_crossentropy', optimizer=Adam(0.0002, 0.5))
    gan.summary()
    return gan

# Function to train the GAN model with early stopping
def train_gan(generators, discriminator, gan, X_trains, y_train, epochs=1000, batch_size=64, patience=10):
    half_batch = int(batch_size / 2)
    history = {"d_loss": [], "g_loss": []}
    best_g_loss = np.inf
    patience_counter = 0

    for epoch in range(epochs):
        # Train Discriminator
        idx = np.random.randint(0, X_trains[0].shape[0], half_batch)
        real_samples = [X[idx] for X in X_trains]
        real_samples = np.concatenate(real_samples, axis=2)
        real_labels = np.ones((half_batch, 1))
        
        noise = [np.random.normal(0, 1, (half_batch, X.shape[1], X.shape[2])) for X in X_trains]
        fake_samples = [gen.predict(n) for gen, n in zip(generators, noise)]
        combined_fake_samples = np.concatenate(fake_samples, axis=2)
        fake_labels = np.zeros((half_batch, 1))
        
        d_loss_real = discriminator.train_on_batch(real_samples, real_labels)
        d_loss_fake = discriminator.train_on_batch(combined_fake_samples, fake_labels)
        d_loss = 0.5 * np.add(d_loss_real, d_loss_fake)
        
        # Train Generators
        noise = [np.random.normal(0, 1, (batch_size, X.shape[1], X.shape[2])) for X in X_trains]
        valid_y = np.ones((batch_size, 1))
        g_loss = gan.train_on_batch(noise, valid_y)
        
        # Save history
        history["d_loss"].append(d_loss)
        history["g_loss"].append(g_loss)
        
        # Early stopping check
        g_loss_value = g_loss if not isinstance(g_loss, list) else g_loss[0]
        if g_loss_value < best_g_loss:
            best_g_loss = g_loss_value
            patience_counter = 0
        else:
            patience_counter += 1
        
        if patience_counter > patience:
            print(f"Early stopping at epoch {epoch}")
            break
        
        if epoch % 100 == 0:
            print(f"{epoch} [D loss: {d_loss}] [G loss: {g_loss}]")

    return history



# Function to save models
def save_models(generators, discriminator, gan, path="models"):
    if not os.path.exists(path):
        os.makedirs(path)
    
    for i, gen in enumerate(generators):
        gen.save(os.path.join(path, f'generator_{i}.keras'))
    
    discriminator.save(os.path.join(path, 'discriminator.keras'))
    gan.save(os.path.join(path, 'gan.keras'))

    print(f"Models saved to {path}")

# Function to load models
def load_models(path="models"):
    generators = []
    for i in range(3):  # Assuming 3 generators
        generators.append(load_model(os.path.join(path, f'generator_{i}.keras')))
    
    discriminator = load_model(os.path.join(path, 'discriminator.keras'))
    gan = load_model(os.path.join(path, 'gan.keras'))

    print(f"Models loaded from {path}")
    return generators, discriminator, gan

# Function to save scalers
def save_scalers(scaler_X, scaler_y, path="models"):
    joblib.dump(scaler_X, os.path.join(path, 'scaler_X.pkl'))
    joblib.dump(scaler_y, os.path.join(path, 'scaler_y.pkl'))
    print(f"Scalers saved to {path}")

# Function to load scalers
def load_scalers(path="models"):
    scaler_X = joblib.load(os.path.join(path, 'scaler_X.pkl'))
    scaler_y = joblib.load(os.path.join(path, 'scaler_y.pkl'))
    print(f"Scalers loaded from {path}")
    return scaler_X, scaler_y


# Function to predict and evaluate the model
def evaluate_gan(generators, X_tests, y_test, scaler):
    y_preds = [gen.predict(X_test) for gen, X_test in zip(generators, X_tests)]
    combined_y_pred = np.concatenate(y_preds, axis=2)
    avg_y_pred = np.mean(combined_y_pred, axis=2, keepdims=True)
    y_pred_rescaled = scaler.inverse_transform(avg_y_pred.reshape(-1, avg_y_pred.shape[-1]))
    
    # Ensure the length of the true values matches the length of the predictions
    y_test = y_test[-len(y_pred_rescaled):]  # Align the end of y_test with y_pred_rescaled
    
    y_test_reshaped = y_test.reshape(-1, y_test.shape[-1])
    y_test_rescaled = scaler.inverse_transform(y_test_reshaped)
    
    y_test_flat = y_test_rescaled.flatten()
    y_pred_flat = y_pred_rescaled.flatten()
    
    min_length = min(len(y_test_flat), len(y_pred_flat))
    y_test_flat = y_test_flat[:min_length]
    y_pred_flat = y_pred_flat[:min_length]
    
    mse = mean_squared_error(y_test_flat, y_pred_flat)
    rmse = np.sqrt(mse)
    
    return y_test_rescaled, y_pred_rescaled, mse, rmse, y_test_flat, y_pred_flat

# Plotting function
def plot_results(y_test, y_pred, history):
    plt.figure(figsize=(10, 5))
    plt.plot(y_test[:len(y_pred)], label='Actual')
    plt.plot(y_pred, label='Predicted')
    plt.legend()
    plt.show()
    
    plt.figure(figsize=(10, 5))
    plt.plot(history["d_loss"], label='Discriminator Loss')
    plt.plot(history["g_loss"], label='Generator Loss')
    plt.legend()
    plt.show()



# For the purpose of this example, we'll generate synthetic data
n_samples = 1000
n_features = 10

X1 = np.random.randn(n_samples, n_features)
X2 = np.random.randn(n_samples, n_features)
X3 = np.random.randn(n_samples, n_features)
y = np.random.randn(n_samples)  # Example target variable

# Ensure all datasets have the same number of samples
assert X1.shape[0] == X2.shape[0] == X3.shape[0] == y.shape[0]

# Create sequences
sequence_length = 10
X_seq1 = create_sequences(X1, sequence_length)
X_seq2 = create_sequences(X2, sequence_length)
X_seq3 = create_sequences(X3, sequence_length)
y_seq = create_sequences(y, sequence_length)

# Split data into training and testing sets
X_train1, X_test1, y_train, y_test = train_test_split(X_seq1, y_seq, test_size=0.2, random_state=42)
X_train2, X_test2, _, _ = train_test_split(X_seq2, y_seq, test_size=0.2, random_state=42)
X_train3, X_test3, _, _ = train_test_split(X_seq3, y_seq, test_size=0.2, random_state=42)

# Scale the data
scaler_X = MinMaxScaler()
scaler_y = MinMaxScaler()

X_train_scaled1 = scaler_X.fit_transform(X_train1.reshape(-1, X_train1.shape[2])).reshape(X_train1.shape)
X_test_scaled1 = scaler_X.transform(X_test1.reshape(-1, X_test1.shape[2])).reshape(X_test1.shape)
X_train_scaled2 = scaler_X.fit_transform(X_train2.reshape(-1, X_train2.shape[2])).reshape(X_train2.shape)
X_test_scaled2 = scaler_X.transform(X_test2.reshape(-1, X_test2.shape[2])).reshape(X_test2.shape)
X_train_scaled3 = scaler_X.fit_transform(X_train3.reshape(-1, X_train3.shape[2])).reshape(X_train3.shape)
X_test_scaled3 = scaler_X.transform(X_test3.reshape(-1, X_test3.shape[2])).reshape(X_test3.shape)
y_train_scaled = scaler_y.fit_transform(y_train.reshape(-1, 1)).reshape(y_train.shape)
y_test_scaled = scaler_y.transform(y_test.reshape(-1, 1)).reshape(y_test.shape)

# Create and compile the models
input_shape = (X_train_scaled1.shape[1], X_train_scaled1.shape[2])

generator1 = create_generator(input_shape)
generator2 = create_generator(input_shape)
generator3 = create_generator(input_shape)

discriminator = create_discriminator((input_shape[0], input_shape[1] * 3))
discriminator.compile(loss='binary_crossentropy', optimizer=Adam(0.0002, 0.5), metrics=['accuracy'])

gan = create_gan([generator1, generator2, generator3], discriminator, input_shape)

# Train the GAN model with early stopping
history = train_gan([generator1, generator2, generator3], discriminator, gan, 
                    [X_train_scaled1, X_train_scaled2, X_train_scaled3], y_train_scaled, 
                    epochs=1000, batch_size=64, patience=10)


# Evaluate the model
y_test_rescaled, y_pred_rescaled, mse, rmse, y_test_flat, y_pred_flat = evaluate_gan([generator1, generator2, generator3], 
                                                           [X_test_scaled1, X_test_scaled2, X_test_scaled3], 
                                                           y_test_scaled, scaler_y)
print(f"MSE: {mse}, RMSE: {rmse}")

# Plot the results
plot_results(y_test_flat, y_pred_flat, history)


# Save the models and scalers
#save_models([generator1, generator2, generator3], discriminator, gan)
#save_scalers(scaler_X, scaler_y)


"""
# Load the models and scalers
generators, discriminator, gan = load_models()
scaler_X, scaler_y = load_scalers()

# Prepare out-of-sample data
X_out_of_sample = np.random.randn(100, n_features)  # Example out-of-sample data
X_seq_out_of_sample = create_sequences(X_out_of_sample, sequence_length)
X_seq_out_of_sample_scaled = scaler_X.transform(X_seq_out_of_sample.reshape(-1, X_seq_out_of_sample.shape[2])).reshape(X_seq_out_of_sample.shape)

# Predict using the loaded models
y_pred_rescaled, mse, rmse = evaluate_gan(generators, 
                                          [X_seq_out_of_sample_scaled] * 3,  # Use the same data for all generators
                                          y_test_scaled[:len(X_seq_out_of_sample_scaled)],  # Dummy target values for evaluation
                                          scaler_y)

print(f"Out-of-sample MSE: {mse}, RMSE: {rmse}")

# Plot the out-of-sample results
plot_results(y_test_scaled[:len(y_pred_rescaled)], y_pred_rescaled, history)
"""