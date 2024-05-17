
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler, StandardScaler
import tensorflow as tf
from keras.models import  Model
from keras.layers import Dense, LSTM, LSTMCell, Dropout, Input,StackedRNNCells, RNN,  Bidirectional, Attention, BatchNormalization
from keras.callbacks import EarlyStopping

from sklearn.metrics import mean_squared_error

tf.config.set_visible_devices([], 'GPU')

data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
# Drop the outputC column
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

timesteps = 60
X, y = create_sequences(df, timesteps)

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=0)
X_train, y_train = np.array(X_train), np.array(y_train) 

inputs = Input(shape=(X_train.shape[1], X_train.shape[2]))

layer1 = 50
layer2 = 50 
rnn_cells_cnt = 50
rnn_range = 2

model = Model()

lstm_out_one = LSTM(layer1,return_sequences=True)(inputs)
dp0 = Dropout(0.3)(lstm_out_one)
lstm_out_two = LSTM(layer2,return_sequences=True)(dp0)

rnn_cells = [LSTMCell(rnn_cells_cnt) for _ in range(rnn_range)]
stacked_rnn = StackedRNNCells(rnn_cells)
stacked_rnn_out = RNN(stacked_rnn)(lstm_out_two)

dp = Dropout(0.2)(stacked_rnn_out)
#output = Dense(1)(dp)  # Change activation and size based on your problem
output = Dense(1)(dp)  # Change activation and size based on your problem

model = Model(inputs=inputs, outputs=output)
model.compile(optimizer='adam', loss='mse')  # Mean Squared Error and Mean Absolute Error as metrics
model.summary()

early_stopping = EarlyStopping(monitor='loss',patience=3)
history = model.fit(X_train, y_train, epochs=100, batch_size=64, validation_split=0.3, callbacks=[early_stopping ])
predictions = model.predict(X_test)

#m_name = f"lstm-{layer1}-{layer2}-{rnn_cells_cnt}-{rnn_range}.keras"
#model.save(m_name)

#ßprint(predictions)


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
