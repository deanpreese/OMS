import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.preprocessing import MinMaxScaler, StandardScaler
from sklearn.model_selection import train_test_split
import tensorflow as tf
from keras.models import Sequential
from keras.layers import LSTM, Dense, Dropout, Input
from keras.callbacks import EarlyStopping

tf.config.set_visible_devices([], 'GPU')

data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
df = data.drop(columns=['outputC'])

# Convert data to sequences
def create_sequences(data, seq_length):
    xs, ys = [], []
    for i in range(len(data) - seq_length):
        x = data.iloc[i:(i + seq_length), :-1]
        y = data.iloc[i + seq_length, -1]  # The 'output' column
        xs.append(x.values)
        ys.append(y)
    return np.array(xs), np.array(ys)

timesteps = 10  # Adjust as necessary for your data
X, y = create_sequences(df, timesteps)

# Normalize each sequence independently
scalers = {}
for i in range(X.shape[0]):
    scalers[i] = MinMaxScaler()
    X[i] = scalers[i].fit_transform(X[i])

# Split the data into training and validation sets
X_train, X_val, y_train, y_val = train_test_split(X, y, test_size=0.2, random_state=0)

layer1 = 100
layer2 = 100
layer3 = 50

# Define the LSTM model
model = Sequential()
model.add( Input(shape=(timesteps, X_train.shape[2])))
model.add(LSTM(layer1, return_sequences=True))
model.add(LSTM(layer2, return_sequences=True))
#model.add(Dropout(0.2))
model.add(LSTM(layer3, return_sequences=False))
#model.add(Dropout(0.2))
model.add(Dense(1))

model.compile(optimizer='adam', loss='mse')
model.summary()

# Early stopping callback
early_stopping = EarlyStopping(monitor='val_loss', patience=10, restore_best_weights=True)

# Train the model
history = model.fit(X_train, y_train, validation_data=(X_val, y_val), epochs=10, batch_size=32, callbacks=[early_stopping])

fig, (ax1, ax2, ax3) = plt.subplots(3, 1, figsize=(16, 9))
fig.suptitle('Vertically stacked subplots')

# Plot training and validation loss
ax1.plot(history.history['loss'], label='Training Loss')
ax1.plot(history.history['val_loss'], label='Validation Loss')
ax1.set_xlabel('Epoch')
ax1.set_ylabel('Loss')
ax1.set_title('Training and Validation Loss')
ax1.grid(True)

# Save the model
#model.save('lstm_model.keras')

# Evaluate the model
val_loss = model.evaluate(X_val, y_val)
print(f'Validation Loss: {val_loss:.4f}')

# Generate predictions
predictions = model.predict(X_val)

# Plot actual vs predicted values
ax2.scatter(y_val, predictions)
ax2.set_xlabel("Actual Output")
ax2.set_ylabel("Predicted Output")
ax2.set_title("Actual vs. Predicted Output")
ax2.grid(True)


# Reverse scaling for predictions
def reverse_scaling(preds, scalers, seq_length):
    reversed_preds = []
    for i in range(len(preds)):
        temp_input = np.zeros((seq_length, len(scalers[i].min_)))
        temp_input[:, -1] = preds[i]
        reversed_pred = scalers[i].inverse_transform(temp_input)
        reversed_preds.append(reversed_pred[0, -1])
    return np.array(reversed_preds)

# Reverse the scaling of predictions
predictions_reversed = reverse_scaling(predictions, scalers, timesteps)

# Plot actual vs predicted values
ax3.plot(range(len(y_val)), y_val, color='blue', label='Actual Values')
ax3.plot(range(len(predictions_reversed)), predictions_reversed, color='red', linestyle='--', label='Predicted Values')
ax3.set_title('Actual vs Predicted Values')
ax3.set_xlabel('Index')
ax3.set_ylabel('Output')
ax2.grid(True)


plt.show()