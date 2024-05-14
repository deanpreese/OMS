import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
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
run_batch_size = 32
run_test_size = 0.3
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
scaler = MinMaxScaler(feature_range=(-1, 1))
#scaler = StandardScaler()
#X_train = scaler.fit_transform(X_train)
#X_test = scaler.transform(X_test)
X_train = np.array(X_train).reshape(X_train.shape[0], 1, X_train.shape[1])
X_test = np.array(X_test).reshape(X_test.shape[0], 1, X_test.shape[1])


units = 32

# Input layer
model = Model()
inputs = Input(shape=(1, input_features))
lstm_out = LSTM(units=units,return_sequences=True,kernel_initializer='glorot_uniform')(inputs)
query = lstm_out
value = lstm_out
attention = Attention(use_scale=True)([query, value])

b1 = Bidirectional(LSTM(units, return_sequences=True))(attention)

rnn_cells = [keras.layers.LSTMCell(units) for _ in range(2)]
stacked_lstm = keras.layers.StackedRNNCells(rnn_cells)
lstm_layer = keras.layers.RNN(stacked_lstm)(b1)

final_output = Flatten()(lstm_layer)  # Flatten the output to connect to a dense layer
output = Dense(1, activation='linear')(lstm_layer)  # Change activation and size based on your problem

model = Model(inputs=inputs, outputs=output)
model.compile(optimizer='adam', loss='binary_crossentropy', metrics=['accuracy'])
model.summary()


"""

regression= Sequential()
regression.add(Input(shape=(1,input_features)))
regression.add(LSTM(units=units,return_sequences=True,kernel_initializer='glorot_uniform',input_shape=(1,input_features)))

#regression.add(Bidirectional(LSTM(units, return_sequences=True)))
#regression.add(Bidirectional(LSTM(units)))

#rnn_cells = [keras.layers.LSTMCell(units) for _ in range(2)]
#stacked_lstm = keras.layers.StackedRNNCells(rnn_cells)
#lstm_layer = keras.layers.RNN(stacked_lstm)
#regression.add(lstm_layer)

attention_layer = Attention()
regression.add(TimeDistributed(attention_layer))  # Wrap with TimeDi

#regression.add(Dropout(0.2))
#regression.add(Attention(use_scale=False, score_mode="dot", dropout=0.2, seed=None))

#regression.add(GRU(4, return_sequences=True, return_state=True))

regression.add(LSTM(units=units,kernel_initializer='glorot_uniform',return_sequences=True))
#regression.add(Dropout(0.2))
#regression.add(LSTM(units=units,kernel_initializer='glorot_uniform'))

hlayer0 = int(input_features * 3)
hlayer1 = int(input_features * 2)
hlayer2 = int(input_features * 2)
hlayer3 = int(input_features * 2)
hlayer4 = int(input_features * 1)

#regression.add(Dense(hlayer2,activation='relu'))
#regression.add(BatchNormalization())
#regression.add(Dropout(0.5))
#regression.add(Dense(hlayer2,activation='relu'))
#regression.add(Dense(hlayer4,activation='relu'))
#regression.add(Dropout(0.2))
#regression.add(Dense(hlayer4,activation='relu'))
#regression.add(BatchNormalization())
regression.add(Dense(1, activation='linear'))
#regression.add(Dense(3, activation="softmax"))



input_dims = X_train.shape[1]
inputs = Input(shape=(1,input_features))
dense1800 = Dense(1800, activation='relu', kernel_regularizer=regularizers.l2(0.01))(inputs)
attention_probs = Dense(1800, activation='sigmoid', name='attention_probs')(dense1800)
attention_mul = multiply([dense1800, attention_probs], name='attention_mul')

# Output layer with 13 units for LSTM input, reshaping might be necessary depending on further usage
outputs = Dense(13, activation='relu', kernel_regularizer=regularizers.l2(0.01))(attention_mul)
# Optional: Reshape to add a time dimension for LSTM input (if needed in your case)
reshaped_output = Reshape((1, 13))(outputs)  # Adds a time-step dimension

# Create the model, using reshaped_output if you plan to connect directly to an LSTM
model = Model(inputs=inputs, outputs=outputs)
#model.compile(optimizer='adam', loss='categorical_crossentropy', metrics=['accuracy'])



model.compile(optimizer='adam',loss='mean_squared_error')

print(" ")
model.summary()
print(" ")
"""

#y_train = np.reshape(y_train, (y_train.size, 1, 13))

history = model.fit(X_train, y_train, epochs=num_epocs, batch_size=run_batch_size, validation_split=0.1, callbacks=[early_stopping])


predictions = model.predict(X_test)
#ShowImportances(data.columns[:input_features],model.feature_importances_, False)
show_stats(False, y_test, predictions)

#RunMSE(y_test, predictions)

"""
# Plot loss and accuracy during training
plt.figure(figsize=(10, 5))
plt.plot(history.history['loss'], label='Training Loss')
plt.plot(history.history['val_loss'], label='Validation Loss')
plt.title('Model Loss')
plt.xlabel('Epoch')
plt.ylabel('Loss')
plt.legend()
plt.show()
"""
