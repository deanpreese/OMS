import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler, StandardScaler
from keras.models import  Model
from keras.layers import Dense, LSTM, Dropout, Input, Bidirectional, Attention 
from keras.callbacks import EarlyStopping

from sklearn.metrics import mean_squared_error

# Read the data into a DataFrame
data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
data.drop(columns=['outputC'])

num_columns = len(data.axes[1]) 
input_features =  num_columns -1
X = data.iloc[:, 0:input_features]  
y = data['output'].values

num_epocs = 100
run_batch_size = 64
run_test_size = 0.8
units=input_features

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=run_test_size, random_state=0)

print("Number of TRAIN patterns:", X_train.shape[0])
print("Size of input patterns:", X_train.shape[1])
print(" ")
print("Number of TEST patterns:", X_test.shape[0])
print("Size of input patterns:", X_test.shape[1])
print(" ")


input_features = X_train.shape[1]

# Rescale the data to the range -1 to 1
#scaler = MinMaxScaler(feature_range=(-1, 1))
scaler = MinMaxScaler()
#scaler = StandardScaler()
X_train = scaler.fit_transform(X_train)
X_test = scaler.transform(X_test)
X_train = np.array(X_train).reshape(X_train.shape[0], 1, X_train.shape[1])
X_test = np.array(X_test).reshape(X_test.shape[0], 1, X_test.shape[1])




model = Model()
inputs = Input(shape=(X_train.shape[1], X_train.shape[2]))
lstm_out = LSTM(units=17,return_sequences=True,kernel_initializer='glorot_uniform')(inputs)
attention = Attention(use_scale=False)([lstm_out, lstm_out])
#bn = BatchNormalization()(attention)
#dp = Dropout(0.5)(bn)
b1 = Bidirectional(LSTM(37, return_sequences=True))(attention)
attention2 = Attention(use_scale=False)([b1, b1])
#b2 = LSTM(32,kernel_initializer='glorot_uniform',return_sequences=False)(attention2)
b2 = Bidirectional(LSTM(37, return_sequences=True))(attention2)

#rnn_cells = [keras.layers.LSTMCell(23) for _ in range(2)]
#stacked_lstm = keras.layers.StackedRNNCells(rnn_cells)
##lstm_layer = keras.layers.RNN(stacked_lstm)(b2)
#lstm_layer = keras.layers.RNN(stacked_lstm)(lstm_out)

d1 = Dense(19,activation='relu')(b2)
#d2 = Dense(17,activation='relu')(d1)

d31 = Dense(9,activation='tanh')(d1)
#d41 = Dense(9,activation='relu')(d31)

d3 = Dense(3,activation='relu')(d31)

#output = Dense(1, activation='linear')(d3)  # Change activation and size based on your problem
output = Dense(1, activation='sigmoid')(d3)  # Change activation and size based on your problem

model = Model(inputs=inputs, outputs=output)
model.compile(optimizer='adam', loss='mse')  # Mean Squared Error and Mean Absolute Error as metrics
model.summary()

early_stopping = EarlyStopping(monitor='loss',patience=3)
history = model.fit(X_train, y_train, epochs=num_epocs, batch_size=run_batch_size, validation_split=0.1, callbacks=[early_stopping ])
predictions = model.predict(X_test)

# Plot loss and accuracy during training
plt.figure(figsize=(10, 5))
plt.plot(history.history['loss'], label='Training Loss')
plt.plot(history.history['val_loss'], label='Validation Loss')
plt.title('Model Loss')
plt.xlabel('Epoch')
plt.ylabel('Loss')
plt.legend()
plt.show()