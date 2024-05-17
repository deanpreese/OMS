import math
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import tensorflow as tf
from tensorflow import keras
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from sklearn.preprocessing import StandardScaler
from sklearn.metrics import mean_squared_error, mean_absolute_error, root_mean_squared_error
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, Input, Attention 
from keras.callbacks import EarlyStopping
from tensorflow.keras.preprocessing.sequence import TimeseriesGenerator

tf.config.set_visible_devices([], 'GPU')

import tensorflow as tf
from tensorflow import keras
import matplotlib.pyplot as plt

# Define constants based on your data

data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
data = data.drop(columns=['outputC'])
data = data.drop(columns=['STOK1'])
data = data.drop(columns=['RSI'])

num_columns = len(data.axes[1]) 
input_features =  num_columns -1
features = data.iloc[:, 0:input_features]  
target = data['output'].values

n_features = input_features
X_train, X_test, y_train, y_test = train_test_split(features, target, test_size=0.2, random_state=0)

X_train = np.array(X_train).reshape(X_train.shape[0], 1, X_train.shape[1])
X_test = np.array(X_test).reshape(X_test.shape[0], 1, X_test.shape[1])

timesteps = 10  # Number of features in each sequence
layer1 = 64
layer2 = 32

# Define the model
model = Sequential()
model.add(Input(shape=(timesteps, n_features)))
model.add(LSTM(layer1, return_sequences=True))
model.add(LSTM(layer2))
model.add(Dense(1))

# Compile the model
model.compile(loss="mse", optimizer="adam")  # Mean Squared Error for regression
model.summary()


# Early stopping callback
early_stopping = EarlyStopping(monitor='val_loss', patience=10, restore_best_weights=True)
# Train the model
#model.fit(X_train, y_train, epochs=10)
history = model.fit(X_train, y_train, validation_data=(X_test, y_test), epochs=10, batch_size=32, callbacks=[early_stopping])


fig, (ax1, ax2) = plt.subplots(2, 1, figsize=(8, 6))
fig.suptitle(f'LSTM Steps {timesteps}   {layer1} {layer2} ')

ax1.plot(history.history['loss'], label='Training Loss')
ax1.plot(history.history['val_loss'], label='Validation Loss')
ax1.set_xlabel('Epoch')
ax1.set_ylabel('Loss')
ax1.set_title('Training and Validation Loss')
ax1.grid(True)


# Evaluate the model on the testing data (optional)
test_loss = model.evaluate(X_test, y_test)
print("Test Loss:", test_loss)

# Make predictions on test data
predicted_values = model.predict(X_test)

# Plot actual vs predicted values
ax2.scatter(y_test, predicted_values)
ax2.set_xlabel("Actual Output")
ax2.set_ylabel("Predicted Output")
ax2.set_title("Actual vs. Predicted Output")
ax2.grid(True)

plt.show()
