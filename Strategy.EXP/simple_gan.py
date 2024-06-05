import numpy as np
import pandas as pd
import tensorflow as tf
import random
from tensorflow.keras.models import Sequential, Model
from tensorflow.keras.layers import Dense, Input, LSTM, TimeDistributed, Reshape, Flatten
from tensorflow.keras.optimizers import Adam
from tensorflow.keras.callbacks import EarlyStopping
from sklearn.preprocessing import MinMaxScaler
from sklearn.metrics import mean_squared_error, mean_absolute_error
from sklearn.model_selection import train_test_split
from scipy.stats import ks_2samp
import matplotlib.pyplot as plt

# Disable GPU usage
tf.config.set_visible_devices([], 'GPU')

def set_seeds(seed=42):
    """Set random seeds for reproducibility."""
    tf.keras.backend.clear_session()
    np.random.seed(seed)
    random.seed(seed)
    tf.random.set_seed(seed)

def create_sequences(data_in, seq_length_in):
    """Create sequences from the input data."""
    print("create_sequences")
    xs = [data_in.iloc[i:i + seq_length_in, :-1].values for i in range(len(data_in) - seq_length_in)]
    ys = data_in.iloc[seq_length_in:, -1].values
    return np.array(xs), np.array(ys)

def normalize_sequences(sequences_in):
    """Normalize sequences using MinMaxScaler."""
    scalers_out = {}
    print("normalize_sequences")
    for i in range(sequences_in.shape[0]):
        scalers_out[i] = MinMaxScaler((0, 1))
        sequences_in[i] = scalers_out[i].fit_transform(sequences_in[i])
    return sequences_in, scalers_out

def normalize_targets(targets_in):
    """Normalize targets using MinMaxScaler."""
    scaler = MinMaxScaler((0, 1))
    print("normalize_targets")
    targets_out = scaler.fit_transform(targets_in.reshape(-1, 1)).flatten()
    return targets_out, scaler

def reverse_scaling(preds_in, scalers_in, seq_length_in, feature_dim_in):
    """Reverse the scaling of the predictions."""
    reversed_preds = []
    for i in range(len(preds_in)):
        temp_input = np.zeros((seq_length_in, feature_dim_in))
        temp_input[:, -1] = preds_in[i]
        reversed_pred = scalers_in[i].inverse_transform(temp_input)
        reversed_preds.append(reversed_pred[0, -1])
    return np.array(reversed_preds)

def sequence_and_normalize(data_in, seq_length_in):
    """Create and normalize sequences, and split the data into training and validation sets."""
    feature_cnt = data_in.shape[1] - 1
    X, y = create_sequences(data_in, seq_length_in)
    X, scalers_x = normalize_sequences(X)
    y, scaler_y = normalize_targets(y)
    X_train, X_val, y_train, y_val = train_test_split(X, y, test_size=0.2, random_state=42)
    return feature_cnt, scalers_x, scaler_y, X_train, X_val, y_train, y_val

def build_generator(timesteps, n_features, latent_dim, layer1, layer2):
    """Define the Generator model."""
    
    input_layer = Input(shape=(latent_dim,))
    x =  Dense(timesteps * n_features, activation="relu")(input_layer)
    x =  Reshape((timesteps, n_features))(x)
    x =  LSTM(layer1, return_sequences=True)(x)
    x =  LSTM(layer2, return_sequences=True)(x)
    output_layer =  TimeDistributed(Dense(n_features))(x)
    
    return Model(input_layer, output_layer)
    
def build_discriminator(timesteps, n_features, layer2, layer1):
    """Define the Discriminator model for time series anomaly detection."""
    
    input_layer = Input(shape=(timesteps, n_features))
    x = LSTM(layer2, return_sequences=True)(input_layer)
    x = LSTM(layer1)(x)
    x = Flatten()(x)
    output_layer =  Dense(1, activation="sigmoid")(x)
    
    return Model(input_layer, output_layer)

def create_gan(generator, discriminator, latent_dim, g_lr=0.001, d_lr=0.001):
    """Create and compile the GAN model."""
    discriminator.compile(loss='binary_crossentropy', optimizer=Adam(learning_rate=d_lr))

    gan_input = Input(shape=(latent_dim,))
    generated_sequence = generator(gan_input)
    discriminator.trainable = False
    gan_output = discriminator(generated_sequence)
    gan_model = Model(gan_input, gan_output)
    gan_model.compile(loss='binary_crossentropy', optimizer=Adam(learning_rate=g_lr))
    gan_model.summary()
    
    return gan_model

def train_model(generator, discriminator, gan_model, X_train, latent_dim, batch_size, epochs, steps_per_epoch, patience_epochs, min_delta=0.001):
    """Train the GAN model."""
    best_g_loss = np.inf
    patience_counter = 0

    history = {'d_loss': [], 'g_loss': []}

    for epoch in range(epochs):
        for step in range(steps_per_epoch):
            noise = tf.random.normal(shape=(batch_size, latent_dim))
            fake_sequences = generator.predict(noise)
            idx = np.random.randint(0, X_train.shape[0], batch_size)
            real_sequences = X_train[idx]

            d_loss_real = discriminator.train_on_batch(real_sequences, np.ones((batch_size, 1)))
            d_loss_fake = discriminator.train_on_batch(fake_sequences, np.zeros((batch_size, 1)))
            d_loss = 0.5 * (d_loss_real + d_loss_fake)

            g_loss = gan_model.train_on_batch(noise, np.ones((batch_size, 1)))

            history['d_loss'].append(d_loss)
            history['g_loss'].append(g_loss)

        print(f"Epoch {epoch}/{epochs} - Discriminator Loss: {d_loss}, Generator Loss: {g_loss[0]}")

        g_loss_value = np.mean(history['g_loss'])
        
        if g_loss_value < best_g_loss - min_delta:
            best_g_loss = g_loss_value
            patience_counter = 0
        else:
            patience_counter += 1

        if patience_counter >= patience_epochs:
            print(f"Early stopping at epoch {epoch}")
            break

        if patience_counter >= patience_epochs:
            print(f"Early stopping at epoch {epoch}")
            break

    return history

def evaluate_model(generator, discriminator, X_test, latent_dim, history, sample_index=0):
    """Evaluate the model and plot results."""
    noise = tf.random.normal(shape=(len(X_test), latent_dim))
    generated_data = generator.predict(noise)

    ks_test = ks_2samp(X_test.flatten(), generated_data.flatten())
    print(f"KS test statistic: {ks_test.statistic}, p-value: {ks_test.pvalue}")

    real_output = discriminator.predict(X_test)
    fake_output = discriminator.predict(generated_data)
    print(f"Discriminator's confidence on real data (mean): {np.mean(real_output)}")
    print(f"Discriminator's confidence on generated data (mean): {np.mean(fake_output)}")

    rmse = np.sqrt(mean_squared_error(X_test.flatten(), generated_data.flatten()))
    mae = mean_absolute_error(X_test.flatten(), generated_data.flatten())
    print(f"Root Mean Squared Error (RMSE) between real and generated data: {rmse}")
    print(f"Mean Absolute Error (MAE) between real and generated data: {mae}")

    plt.show()

def plot_results(generator, discriminator, X_test, latent_dim, history, sample_index=0):
    """Evaluate the model and plot results."""
    noise = tf.random.normal(shape=(len(X_test), latent_dim))
    generated_data = generator.predict(noise)

    fig, axs = plt.subplots(2, 2, figsize=(16, 9))
    
    axs[0, 0].plot(X_test[sample_index], label='Actual', linestyle='--', marker='o')
    axs[0, 0].plot(generated_data[sample_index], label='Generated', linestyle='--', marker='x')
    axs[0, 0].set_title('Actual vs. Predicted Sequence')
    axs[0, 0].set_xlabel('Timesteps')
    axs[0, 0].set_ylabel('Normalized Values')
    
    actual_softmax = tf.nn.softmax(X_test[sample_index], axis=-1)
    generated_softmax = tf.nn.softmax(generated_data[sample_index], axis=-1)
    
    axs[0, 1].plot(actual_softmax, label='Actual Softmax', linestyle='--', marker='o')
    axs[0, 1].plot(generated_softmax, label='Generated Softmax', linestyle='--', marker='x')
    axs[0, 1].set_title('Softmax of Actual vs. Predicted Sequence')
    axs[0, 1].set_xlabel('Timesteps')
    axs[0, 1].set_ylabel('Softmax Values')
    
    avg_actual = np.mean(X_test, axis=0)
    avg_generated = np.mean(generated_data, axis=0)
    
    axs[1, 1].plot(avg_actual, label='Averaged Actual', linestyle='--', marker='o')
    axs[1, 1].plot(avg_generated, label='Averaged Generated', linestyle='--', marker='x')
    axs[1, 1].set_title('Averaged Actual vs. Predicted Sequence')
    axs[1, 1].set_xlabel('Timesteps')
    axs[1, 1].set_ylabel('Normalized Values')
    
    axs[1, 0].plot(history['d_loss'], label='Discriminator Loss')
    axs[1, 0].plot(history['g_loss'], label='Generator Loss')
    axs[1, 0].set_title('Training Loss')
    axs[1, 0].set_xlabel('Steps')
    axs[1, 0].set_ylabel('Loss')

    plt.show()

def run():
    set_seeds(42)

    # Define constants
    timesteps_in = 13  # Number of timesteps in each sequence
    latent_dim_in = 7  # Dimension of the latent space
    batch_size_in = 32
    epochs_in = 100
    steps_per_epoch_in = 10
    patience_in = 15
    lay1 = 128
    lay2 = 64
    g_learn = 0.0001
    d_learn = 0.0001

    # Paths to data and models
    training_data_path = 'data/buildSeqInd_Lucky13_5M_ALL.csv'

    # Load and preprocess the data
    training_data = pd.read_csv(training_data_path)
    training_data = training_data.drop(columns=['outputC', 'outputX'])

    cnt_features, x_scalers, y_scaler, X_train, X_val, y_train, y_val = sequence_and_normalize(training_data, timesteps_in)
    out_dim = cnt_features

    generator = build_generator(timesteps_in, cnt_features, latent_dim_in, lay1, lay2)
    discriminator = build_discriminator(timesteps_in, cnt_features, lay2, lay1)
    gan_model = create_gan(generator, discriminator, latent_dim_in, g_learn, d_learn)

    # Train the model
    history = train_model(generator, discriminator, gan_model, X_train, latent_dim_in, batch_size_in, epochs_in, steps_per_epoch_in, patience_in)

    # Evaluate the model
    evaluate_model(generator, discriminator, X_val, latent_dim_in, history, sample_index=0)

if __name__ == "__main__":
    run()
