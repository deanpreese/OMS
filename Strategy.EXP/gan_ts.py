import numpy as np
import pandas as pd
import tensorflow as tf
from tensorflow import keras
from tensorflow.keras import layers
from sklearn.preprocessing import MinMaxScaler
import matplotlib.pyplot as plt
from sklearn.metrics import mean_squared_error, mean_absolute_error
from scipy.stats import ks_2samp

# Save and Load Functions
def save_model(model, filename):
    model.save(filename)

def load_model(filename):
    return keras.models.load_model(filename)

# Function to preprocess data
def preprocess_data(df, timesteps):
    features = df.iloc[:, :-1].values  # All columns except the last one as features
    target = df.iloc[:, -1].values.reshape(-1, 1)  # Last column as target

    # Normalize the data
    scaler_features = MinMaxScaler(feature_range=(0, 1))
    scaler_target = MinMaxScaler(feature_range=(0, 1))

    scaled_features = scaler_features.fit_transform(features)
    scaled_target = scaler_target.fit_transform(target)

    # Create sequences of data
    X, y = [], []
    for i in range(timesteps, len(scaled_features)):
        X.append(scaled_features[i-timesteps:i])
        y.append(scaled_target[i])
    
    X, y = np.array(X), np.array(y)
    
    return X, y, scaler_features, scaler_target

# Define the Generator model
def build_generator(timesteps, n_features, latent_dim, output_dim, layer1, layer2):
    model = keras.Sequential()
    model.add(layers.Input(shape=(latent_dim,)))
    model.add(layers.Dense(timesteps * n_features, activation="relu"))
    model.add(layers.Reshape((timesteps, n_features)))
    model.add(layers.LSTM(layer1, return_sequences=True))
    model.add(layers.LSTM(layer2, return_sequences=True))  # Ensure output has 3 dimensions
    model.add(layers.TimeDistributed(layers.Dense(output_dim)))  # Use TimeDistributed to output a sequence
    return model

# Define the Discriminator model (for time series anomaly detection)
def build_discriminator(timesteps, n_features , layer1, layer2):
    model = keras.Sequential()
    model.add(layers.Input(shape=(timesteps, n_features)))
    model.add(layers.LSTM(layer1, return_sequences=True))
    model.add(layers.LSTM(layer2))
    model.add(layers.Flatten())
    model.add(layers.Dense(1, activation="sigmoid"))
    return model

# Training function
def train_model(df, timesteps, n_features, latent_dim, batch_size, epochs, steps_per_epoch, patience_epochs, layer1, layer2):

    # Preprocess data
    X_train, y_train, scaler_features, scaler_target = preprocess_data(df, timesteps)
    train_size = int(X_train.shape[0] * 0.8)
    X_train, X_test = X_train[:train_size], X_train[train_size:]
    y_train, y_test = y_train[:train_size], y_train[train_size:]

    # Build Generator and Discriminator
    generator = build_generator(timesteps, n_features, latent_dim, n_features, layer1, layer2)
    discriminator = build_discriminator(timesteps, n_features, layer1, layer2)

    # Define the combined GAN model for training (no training on the combined model directly)
    gan_input = layers.Input(shape=(latent_dim,))
    generated_sequence = generator(gan_input)
    gan_output = discriminator(generated_sequence)
    gan_model = keras.Model(gan_input, gan_output)

    # Loss functions (Binary Cross Entropy for Discriminator, Mean Squared Error for Generator)
    discriminator_loss_fn = tf.keras.losses.BinaryCrossentropy(from_logits=True)
    generator_loss_fn = tf.keras.losses.MeanSquaredError()

    # Optimizers
    generator_optimizer = tf.keras.optimizers.Adam(learning_rate=0.001)
    discriminator_optimizer = tf.keras.optimizers.Adam(learning_rate=0.001)

    # Compile the Discriminator model separately (trains to distinguish real from fake)
    discriminator.compile(loss=discriminator_loss_fn, optimizer=discriminator_optimizer)
    discriminator.summary()

    # Early stopping callback
    early_stopping = keras.callbacks.EarlyStopping(
        monitor='gen_loss', patience=patience_epochs, restore_best_weights=True)

    # History collection
    history = {'d_loss': [], 'g_loss': []}

    # Training loop
    for epoch in range(epochs):
        for step in range(steps_per_epoch):
            # Sample random noise for the generator
            noise = tf.random.normal(shape=(batch_size, latent_dim))

            # Generate fake sequences
            fake_sequences = generator(noise)

            # Sample a batch of real sequences
            idx = np.random.randint(0, X_train.shape[0], batch_size)
            real_sequences = X_train[idx]

            # Train Discriminator (maximize the probability of distinguishing real from fake)
            with tf.GradientTape() as tape:
                real_output = discriminator(real_sequences)
                fake_output = discriminator(fake_sequences)
                real_loss = discriminator_loss_fn(tf.ones_like(real_output), real_output)
                fake_loss = discriminator_loss_fn(tf.zeros_like(fake_output), fake_output)
                total_loss = 0.5 * (real_loss + fake_loss)
            gradients = tape.gradient(total_loss, discriminator.trainable_variables)
            discriminator_optimizer.apply_gradients(zip(gradients, discriminator.trainable_variables))

            # Train Generator (minimize the Discriminator loss)
            with tf.GradientTape() as tape:
                generated_sequences = generator(noise)
                fake_output = discriminator(generated_sequences)
                gen_loss = discriminator_loss_fn(tf.ones_like(fake_output), fake_output)
            gradients = tape.gradient(gen_loss, generator.trainable_variables)
            generator_optimizer.apply_gradients(zip(gradients, generator.trainable_variables))

            # Record history
            history['d_loss'].append(total_loss.numpy())
            history['g_loss'].append(gen_loss.numpy())

            # Debugging: Print tensor shapes
            if step == 0 or step == 5:
                print(f"Epoch {epoch}, Step {step} G: {gen_loss}  D:  {total_loss}  ")

        # Optionally save the model periodically
        #if epoch % 100 == 0:
        #    save_model(generator, f"m_data/generator_epoch_{epoch}.keras")
        #    save_model(discriminator, f"m_data/discriminator_epoch_{epoch}.keras")

        # Check for early stopping
        if early_stopping.stopped_epoch > 0:
            print(f"Early stopping at epoch {epoch}")
            break

    return scaler_features, scaler_target, generator, discriminator, history, X_test


# Evaluation function
def evaluate_model(generator, discriminator, X_test, batch_size, latent_dim, history, sample_index=0):
    # Generate synthetic data
    noise = tf.random.normal(shape=(len(X_test), latent_dim))
    generated_data = generator.predict(noise)

    fig, axs = plt.subplots(2, 2, figsize=(16, 9))
    
    axs[0,0].plot(X_test[sample_index], label='Actual', linestyle='--', marker='o')
    axs[0,0].plot(generated_data[sample_index], label='Generated', linestyle='--', marker='x')
    axs[0,0].set_title('Actual vs. Predicted Sequence')
    axs[0,0].set_xlabel('Timesteps')
    axs[0,0].set_ylabel('Normalized Values')
    
    softmax = keras.activations.softmax
    actual_softmax = softmax(X_test[sample_index], axis=-1)
    generated_softmax = softmax(generated_data[sample_index], axis=-1)
    
    axs[0,1].plot(actual_softmax, label='Actual Softmax', linestyle='--', marker='o')
    axs[0,1].plot(generated_softmax, label='Generated Softmax', linestyle='--', marker='x')
    axs[0,1].set_title('Softmax of Actual vs. Predicted Sequence')
    axs[0,1].set_xlabel('Timesteps')
    axs[0,1].set_ylabel('Softmax Values')
    
    avg_actual = np.mean(X_test, axis=0)
    avg_generated = np.mean(generated_data, axis=0)
    
    axs[1,1].plot(avg_actual, label='Averaged Actual', linestyle='--', marker='o')
    axs[1,1].plot(avg_generated, label='Averaged Generated', linestyle='--', marker='x')
    axs[1,1].set_title('Averaged Actual vs. Predicted Sequence')
    axs[1,1].set_xlabel('Timesteps')
    axs[1,1].set_ylabel('Normalized Values')
    
    axs[1,0].plot(history['d_loss'], label='Discriminator Loss')
    axs[1,0].plot(history['g_loss'], label='Generator Loss')
    axs[1,0].set_title('Training Loss')
    axs[1,0].set_xlabel('Steps')
    axs[1,0].set_ylabel('Loss')
    
    
    # Statistical similarity
    real_mean = np.mean(X_test, axis=0)
    gen_mean = np.mean(generated_data, axis=0)
    #print(f"Average values of features (Real data): {real_mean}")
    #print(f"Average values of features (Generated data): {gen_mean}")

    real_var = np.var(X_test, axis=0)
    gen_var = np.var(generated_data, axis=0)
    #print(f"Spread of feature values (Variance) - Real data: {real_var}")
    #print(f"Spread of feature values (Variance) - Generated data: {gen_var}")

    # Kolmogorov-Smirnov test
    ks_test = ks_2samp(X_test.flatten(), generated_data.flatten())
    print(f"KS test statistic: {ks_test.statistic}, p-value: {ks_test.pvalue}")

    # Discriminator performance
    real_output = discriminator.predict(X_test)
    fake_output = discriminator.predict(generated_data)
    print(f"Discriminator's confidence on real data (mean): {np.mean(real_output)}")
    print(f"Discriminator's confidence on generated data (mean): {np.mean(fake_output)}")

    # Evaluation metrics
    rmse = np.sqrt(mean_squared_error(X_test.flatten(), generated_data.flatten()))
    mae = mean_absolute_error(X_test.flatten(), generated_data.flatten())
    print(f"Root Mean Squared Error (RMSE) between real and generated data: {rmse}")
    print(f"Mean Absolute Error (MAE) between real and generated data: {mae}")

    plt.show()



# Define constants
timesteps_in = 21  # Number of timesteps in each sequence
n_features_in = 13  # Number of features excluding output
latent_dim_in = 5  # Dimension of the latent space
batch_size_in = 32
epochs_in = 1000
steps_per_epoch_in = 10
patience_in = 15
sample_index = 0
lay1 = 17
lay2 = 9

# Paths to data and models
training_data_path = 'data/buildSeqInd_Lucky13_5M_ALL.csv'
#training_data_path = 'data/sm13_3070.csv'

new_data_path = 'data/new_market_data.csv'
generator_model_path = 'm_data/generator_final.keras'
discriminator_model_path = 'm_data/discriminator_final.keras'

# Load the training data into a DataFrame and drop the unwanted column
training_data = pd.read_csv(training_data_path)
training_data = training_data.drop(columns=['outputC'])

# Train the model
scaler_features, scaler_target, generator, discriminator, history, X_test= train_model(training_data, timesteps_in, n_features_in, 
                        latent_dim_in, batch_size_in, epochs_in, steps_per_epoch_in, patience_in, lay1, lay2)

evaluate_model(generator, discriminator, X_test, batch_size_in, latent_dim_in, history, sample_index=0)
#evaluate_model(generator, discriminator, X_test, batch_size_in, latent_dim_in, history, sample_index=1)

save_model(generator, "generator_final.keras")
save_model(discriminator, "discriminator_final.keras")

# Perform loop prediction
#predictions = loop_predict(generator_model_path, new_data_path, scaler_features, scaler_target, timesteps, n_features, latent_dim)

#Print all predictions
#print("All Predictions:")
#for prediction in predictions:
#    print(prediction)
