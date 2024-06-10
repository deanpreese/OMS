import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from sklearn.preprocessing import MinMaxScaler
import tensorflow as tf
from tensorflow import keras
from tensorflow.keras.layers import Dense, Dropout, Input, Concatenate, Lambda, LSTM
from tensorflow.keras.models import Model
from tensorflow.keras.optimizers import Adam
from tensorflow.keras import backend as K

# Data Loading and Preprocessing
def load_and_preprocess_data(file_path, seq_len=50, future_len=10):

    data = pd.read_csv(file_path)
    
    #data = data.drop(columns=drop_cols)
    data = data.drop(columns=['outputC'])
    
    X_data = data.drop(columns=['output'])    
    y_data = data['output'].values.reshape(-1, 1)

    scaler_input = MinMaxScaler()
    scaler_output = MinMaxScaler()

    input_data = scaler_input.fit_transform(X_data.values)
    output_data = scaler_output.fit_transform(y_data)

    X, y = [], []
    for i in range(len(input_data) - seq_len - future_len):
        X.append(input_data[i:i+seq_len])
        y.append(output_data[i+seq_len:i+seq_len+future_len])

    X, y = np.array(X), np.array(y)
    return X, y, scaler_input, scaler_output

# Transformer Encoder
class TransformerEncoder(tf.keras.layers.Layer):
    def __init__(self, embed_dim, dense_dim, num_heads, **kwargs):
        super(TransformerEncoder, self).__init__(**kwargs)
        self.embed_dim = embed_dim
        self.dense_dim = dense_dim
        self.num_heads = num_heads
        self.attention = tf.keras.layers.MultiHeadAttention(num_heads=num_heads, key_dim=embed_dim)
        self.dense_proj = tf.keras.Sequential([tf.keras.layers.Dense(dense_dim, activation='relu'),
                                               tf.keras.layers.Dense(embed_dim)])

    def call(self, inputs):
        inputs = inputs[:, tf.newaxis]
        attention_output = self.attention(inputs, inputs)
        proj_output = self.dense_proj(attention_output)
        return proj_output

# Transformer Decoder
class TransformerDecoder(tf.keras.layers.Layer):
    def __init__(self, embed_dim, dense_dim, num_heads, **kwargs):
        super(TransformerDecoder, self).__init__(**kwargs)
        self.embed_dim = embed_dim
        self.dense_dim = dense_dim
        self.num_heads = num_heads
        self.attention_1 = tf.keras.layers.MultiHeadAttention(num_heads=num_heads, key_dim=embed_dim)
        self.attention_2 = tf.keras.layers.MultiHeadAttention(num_heads=num_heads, key_dim=embed_dim)
        self.dense_proj = tf.keras.Sequential([tf.keras.layers.Dense(dense_dim, activation='relu'),
                                               tf.keras.layers.Dense(embed_dim)])

    def call(self, inputs, encoder_outputs):
        inputs = inputs[:, tf.newaxis]
        attention_output_1 = self.attention_1(inputs, inputs)
        attention_output_2 = self.attention_2(encoder_outputs, attention_output_1)
        proj_output = self.dense_proj(attention_output_2)
        return proj_output

# Adversarial Training
def wasserstein_loss(y_true, y_pred):
    return K.mean(y_true * y_pred)

def gradient_penalty(batch_size, real_data, fake_data):
    alpha = tf.random.uniform((batch_size, 1, 1))
    interpolated = alpha * real_data + (1 - alpha) * fake_data

    with tf.GradientTape() as gp_tape:
        gp_tape.watch(interpolated)
        pred = discriminator(interpolated)
        grads = gp_tape.gradient(pred, [interpolated])[0]
        grads_norm = tf.sqrt(tf.reduce_sum(tf.square(grads), axis=[1, 2]))
        gp = tf.reduce_mean((grads_norm - 1.0) ** 2)

    return gp

# Model Architecture
def build_transformer_model(seq_len, future_len, input_dim, embed_dim=64, dense_dim=128, num_heads=4):
    encoder_inputs = Input(shape=(seq_len, input_dim))
    encoder = TransformerEncoder(embed_dim, dense_dim, num_heads)(encoder_inputs)

    decoder_inputs = Input(shape=(future_len, 1, 1))  # Add an extra dimension for the number of features
    #decoder_inputs = Input(shape=(future_len, 1))
    decoder = TransformerDecoder(embed_dim, dense_dim, num_heads)(decoder_inputs, encoder)
    decoder_outputs = Dense(1)(decoder)

    model = Model(inputs=[encoder_inputs, decoder_inputs], outputs=decoder_outputs)
    return model

# Discriminator Architecture
def build_discriminator(seq_len, future_len, input_dim):
    inputs = Input(shape=(seq_len + future_len, input_dim))
    x = LSTM(64, return_sequences=True)(inputs)
    x = Dropout(0.2)(x)
    x = LSTM(32)(x)
    x = Dropout(0.2)(x)
    outputs = Dense(1, activation='linear')(x)
    model = Model(inputs=inputs, outputs=outputs)
    return model

# Training
def train_transformer_gan(X_train, y_train, seq_len, future_len, input_dim, epochs, batch_size):
    transformer = build_transformer_model(seq_len, future_len, input_dim)
    discriminator = build_discriminator(seq_len, future_len, input_dim)

    opt_transformer = Adam(0.0002, 0.5)
    opt_discriminator = Adam(0.0002, 0.5)

    transformer.compile(optimizer=opt_transformer, loss='mse')
    discriminator.compile(optimizer=opt_discriminator, loss=wasserstein_loss)
    transformer.summary()
    discriminator.summary()
    

    for epoch in range(epochs):
        # Train Transformer
        transformer.trainable = True
        discriminator.trainable = False
        for batch in range(len(X_train) // batch_size):
            X_batch = X_train[batch * batch_size:(batch + 1) * batch_size]
            y_batch = y_train[batch * batch_size:(batch + 1) * batch_size]
            transformer_loss = transformer.train_on_batch([X_batch, y_batch[:, :-1]], y_batch[:, 1:])

        # Train Discriminator
        transformer.trainable = False
        discriminator.trainable = True
        for batch in range(len(X_train) // batch_size):
            X_batch = X_train[batch * batch_size:(batch + 1) * batch_size]
            y_batch = y_train[batch * batch_size:(batch + 1) * batch_size]
            real_data = np.concatenate([X_batch, y_batch], axis=2)
            fake_data = discriminator.predict(transformer.predict([X_batch, y_batch[:, :-1]]))
            discriminator_loss = discriminator.train_on_batch(real_data, -np.ones((batch_size, 1)))
            discriminator_loss += discriminator.train_on_batch(fake_data, np.ones((batch_size, 1)))
            discriminator_loss += gradient_penalty(batch_size, real_data, fake_data)

        print(f'Epoch {epoch+1}/{epochs}, Transformer Loss: {transformer_loss}, Discriminator Loss: {discriminator_loss}')

    return transformer, discriminator

# Evaluation
def evaluate_model(model, X_test, y_test, scaler_output):
    
    y_pred = model.predict([X_test, np.zeros_like(y_test[:, :-1, np.newaxis])])
    #y_pred = model.predict([X_test, np.zeros_like(y_test[:, :-1])])
    y_pred = scaler_output.inverse_transform(y_pred.reshape(-1, 1))
    y_true = scaler_output.inverse_transform(y_test[:, 1:].reshape(-1, 1))
    mse = np.mean((y_pred - y_true)**2)
    return mse

# Visualization
def plot_predictions(y_true, y_pred):
    plt.figure(figsize=(12, 6))
    plt.plot(y_true, label='True Values')
    plt.plot(y_pred, label='Predicted Values')
    plt.title('Stock Price Predictions')
    plt.xlabel('Time')
    plt.ylabel('Price')
    plt.legend()
    plt.show()

# Main Function
def main():
    
    data_file = "data/buildSeqInd_Lucky13_5M_ALL.csv"
    X, y, scaler_input, scaler_output = load_and_preprocess_data(data_file, seq_len=50, future_len=10)

    # Split data into train and test sets
    train_size = int(len(X) * 0.8)
    X_train, X_test = X[:train_size], X[train_size:]
    y_train, y_test = y[:train_size], y[train_size:]

    # Train the model
    transformer, discriminator = train_transformer_gan(X_train, y_train, seq_len=50, future_len=10,
                                                       input_dim=X.shape[-1], epochs=100, batch_size=32)

    # Evaluate the model
    mse = evaluate_model(transformer, X_test, y_test, scaler_output)
    print(f'Mean Squared Error: {mse}')

    # Make predictions
    y_pred = transformer.predict([X_test, np.zeros_like(y_test[:, :-1])])
    y_pred = scaler_output.inverse_transform(y_pred.reshape(-1, 1))
    y_true = scaler_output.inverse_transform(y_test[:, 1:].reshape(-1, 1))

    # Visualize predictions
    plot_predictions(y_true, y_pred)

if __name__ == '__main__':
    main()