import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import tensorflow as tf
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from sklearn.preprocessing import StandardScaler

import datetime

import keras
from keras import regularizers
from keras.models import Sequential, Model
from keras.layers import Dense, LSTM, Dropout, Input, Bidirectional, BatchNormalization, Attention,  multiply, Reshape, Flatten, AdditiveAttention
from sklearn.ensemble import RandomForestRegressor
from keras.callbacks import EarlyStopping, TensorBoard

early_stopping = EarlyStopping(monitor='loss',patience=5)
from common.common_func import gen_importances, show_stats, calc_MSE
from sklearn.metrics import mean_squared_error


# Read the data into a DataFrame
data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')

# Drop 'outputC' column
data = data.drop(columns=['outputC'])

# Normalize the data
scaler = MinMaxScaler()
scaled_data = scaler.fit_transform(data)

# Convert to sequences for LSTM
def create_sequences(data, sequence_length):
    X, y = [], []
    for i in range(len(data) - sequence_length):
        X.append(data[i:i+sequence_length, :-1])
        y.append(data[i+sequence_length, -1])
    return np.array(X), np.array(y)

sequence_length = 2
X, y = create_sequences(scaled_data, sequence_length)

# Split into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.3, random_state=0)

# Build the LSTM model
model = Sequential()
model.add(LSTM(25, return_sequences=True, input_shape=(X_train.shape[1], X_train.shape[2])))
model.add(Dropout(0.2))
model.add(LSTM(15))
model.add(Dropout(0.2))
model.add(Dense(1))

# Compile the model
model.compile(optimizer='SGD', loss='mean_squared_error')
model.summary()

# Early stopping callback
early_stopping = EarlyStopping(monitor='val_loss', patience=5, restore_best_weights=True)

# Train the model
history = model.fit(X_train, y_train, epochs=100, batch_size=64, validation_split=0.3, callbacks=[early_stopping])

# Evaluate the model
loss = model.evaluate(X_test, y_test)
print('Test loss:', loss)

model.save('lstm-25-15.keras')

# Plot loss and accuracy during training
plt.figure(figsize=(10, 5))
plt.plot(history.history['loss'], label='Training Loss')
plt.plot(history.history['val_loss'], label='Validation Loss')
plt.title('Model Loss')
plt.xlabel('Epoch')
plt.ylabel('Loss')
plt.legend()
plt.show()