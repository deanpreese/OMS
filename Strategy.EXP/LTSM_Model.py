import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from sklearn.preprocessing import StandardScaler
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, Input, Embedding, Bidirectional,TimeDistributed, BatchNormalization, Conv1D  
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

units = 11
regression= Sequential()
regression.add(LSTM(units=units,return_sequences=True,kernel_initializer='glorot_uniform',input_shape=(1,input_features)))
regression.add(Dropout(0.2))
regression.add(LSTM(units=units,return_sequences=True))
#regression.add(LSTM(units=units,kernel_initializer='glorot_uniform',return_sequences=True))
regression.add(Dropout(0.2))
#regression.add(LSTM(units=units,kernel_initializer='glorot_uniform',return_sequences=True))
#regression.add(LSTM(units=units,return_sequences=False))
#regression.add(Dropout(0.2))
#regression.add(LSTM(units=units,kernel_initializer='glorot_uniform'))
#regression.add(Dropout(0.2))
regression.add(Dense(units=64))
regression.add(Dropout(0.2))
#regression.add(Dense(units=1))

hlayer0 = int(input_features * 5)
hlayer1 = int(input_features * 5)
hlayer2 = int(input_features * 6)
hlayer3 = int(input_features * 3)
hlayer4 = int(input_features * 1)

#regression.add(Conv1D(filters=32,kernel_size=(units,),activation='relu'))
regression.add(Bidirectional(LSTM(32, return_sequences=True)))
regression.add(Bidirectional(LSTM(32)))    

#regression.add(Input(shape=(1,input_features),))
#regression.add(Dense(hlayer0,activation='relu'))
regression.add(BatchNormalization())
regression.add(Dropout(0.2))
regression.add(Dense(hlayer1,activation='relu'))
#regression.add(Dense(hlayer2,activation='relu'))
regression.add(BatchNormalization())
regression.add(Dropout(0.2))
#regression.add(Dense(hlayer3,activation='relu'))
#regression.add(Dropout(0.2))
regression.add(Dense(hlayer4,activation='relu'))
#regression.add(BatchNormalization())
regression.add(Dense(1, activation='linear'))


model = regression

model.compile(optimizer='adam',loss='mean_squared_error')

print(" ")
model.summary()
print(" ")

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

