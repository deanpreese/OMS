import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import tensorflow as tf
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from sklearn.preprocessing import StandardScaler

import keras
from keras import regularizers
from keras.models import Sequential, Model
from keras.layers import Dense, LSTM, Dropout, Input, Embedding, Bidirectional,TimeDistributed, BatchNormalization, Attention, GRU, multiply, Reshape, Flatten
from sklearn.ensemble import RandomForestRegressor
from keras.callbacks import EarlyStopping

early_stopping = EarlyStopping(monitor='loss',patience=5)
from common.common_func import gen_importances, show_stats, calc_MSE
from sklearn.metrics import mean_squared_error


#data = pd.read_csv("data/Seq_30_R2080_TAG.csv")   #83%ß
#data = pd.read_csv("data/Seq_30_R3070_TAG.csv")   #75%   
#data = pd.read_csv("data/Seq_TopSet_R3070_TAG.csv")   #  
#data = pd.read_csv("data/Seq_Ind_R3070_TAG.csv")   #  

data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')

num_columns = len(data.columns)
input_features = num_columns - 2
X = data.iloc[:, 0:input_features]
y = data['output'].values

num_epocs = 100
run_batch_size = 64
run_test_size = 0.7

scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)
X_train, X_test, y_train, y_test = train_test_split(X_scaled, y, test_size=run_test_size, random_state=0)

print("Number of TRAIN patterns:", X_train.shape[0])
print("Size of input patterns:", X_train.shape[1])
print(" ")
print("Number of TEST patterns:", X_test.shape[0])
print("Size of input patterns:", X_test.shape[1])
print(" ")


tf.config.set_visible_devices([], 'GPU')


tf_model = Sequential()
tf_model.Add(Input(shape=(X_train.shape[1],1)))

#tf_model.Add(LSTM(units=78,return_sequences=True,kernel_initializer='glorot_uniform'))
tf_model.Add(LSTM(units=78,return_sequences=False,kernel_initializer='glorot_uniform'))

tf_model.Add(Attention(use_scale=True))
tf_model.Add(BatchNormalization())
tf_model.Add(Dropout(0.5))

#tf_model.Add(Bidirectional(LSTM(39, return_sequences=True)))
tf_model.Add(Bidirectional(LSTM(39, return_sequences=False)))

tf_model.Add(Attention(use_scale=False))

#tf_model.Add(LSTM(32,kernel_initializer='glorot_uniform',return_sequences=True))
tf_model.Add(LSTM(32,kernel_initializer='glorot_uniform',return_sequences=False))

#rnn_cells = [keras.layers.LSTMCell(units) for _ in range(2)]
#stacked_lstm = keras.layers.StackedRNNCells(rnn_cells)
#lstm_layer = keras.layers.RNN(stacked_lstm)(l2)

#d1 = Dense(100,activation='relu')(lstm_layer)
#d2 = Dense(100,activation='relu')(d1)


tf_model.Add(Dense(13,activation='softmax'))

#tf_model.Add(Dense(1, activation='linear'))
tf_model.Add(Dense(1, activation='sigmoid'))

tf_model.compile(optimizer='adam', loss='mse', metrics=['mae','accuracy'])
tf_model.summary()


"""

# Input layer
model = Model()
#inputs = Input(shape=(1, input_features))
inputs = Input(shape=(X_train.shape[1],1))
lstm_out = LSTM(units=78,return_sequences=True,kernel_initializer='glorot_uniform')(inputs)
attention = Attention(use_scale=True)([lstm_out, lstm_out])
bn = BatchNormalization()(attention)
dp = Dropout(0.5)(bn)
b1 = Bidirectional(LSTM(39, return_sequences=True))(dp)
attention2 = Attention(use_scale=False)([b1, b1])
b2 = LSTM(32,kernel_initializer='glorot_uniform',return_sequences=True)(attention2)
#rnn_cells = [keras.layers.LSTMCell(units) for _ in range(2)]
#stacked_lstm = keras.layers.StackedRNNCells(rnn_cells)
#lstm_layer = keras.layers.RNN(stacked_lstm)(l2)

#d1 = Dense(100,activation='relu')(lstm_layer)
#d2 = Dense(100,activation='relu')(d1)

d3 = Dense(13,activation='softmax')(b2)
#output = Dense(1, activation='linear')(d3)  # Change activation and size based on your problem
output = Dense(1, activation='sigmoid')(d3)  # Change activation and size based on your problem

model = Model(inputs=inputs, outputs=output)
model.compile(optimizer='adam', loss='mse', metrics=['mae','accuracy'])  # Mean Squared Error and Mean Absolute Error as metrics
model.summary()

"""


model = tf_model

history = model.fit(X_train, y_train, epochs=num_epocs, batch_size=run_batch_size, validation_split=0.1, callbacks=[early_stopping])
predictions = model.predict(X_test)
#ShowImportances(data.columns[:input_features],model.feature_importances_, False)
show_stats(False, y_test, predictions)

#RunMSE(y_test, predictions)

# Plot loss and accuracy during training
plt.figure(figsize=(10, 5))
plt.plot(history.history['loss'], label='Training Loss')
plt.plot(history.history['val_loss'], label='Validation Loss')
plt.title('Model Loss')
plt.xlabel('Epoch')
plt.ylabel('Loss')
plt.legend()
plt.show()
