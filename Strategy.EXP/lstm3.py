import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler, StandardScaler
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, Input, Embedding, Bidirectional,TimeDistributed, Attention 
from keras.callbacks import EarlyStopping

from sklearn.metrics import mean_squared_error

# Read the data into a DataFrame
data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
data = data.drop(columns=['outputC'])

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

layer1 = 90
layer2= 65
layer3= 8


# Build the LSTM model
model = Sequential()
model.add(Input(shape = (X_train.shape[1], X_train.shape[2])))
model.add(LSTM(layer1, return_sequences=True))
model.add(Dropout(0.3))
model.add(Dropout(0.2))
model.add(LSTM(layer2))
model.add(Dropout(0.2))
model.add(Dense(layer3))
model.add(Dropout(0.2))
model.add(Dense(1))



model.compile(optimizer='adam',loss='mean_squared_error')

print(" ")
model.summary()
print(" ")

early_stopping = EarlyStopping(monitor='loss',patience=3)

history = model.fit(X_train, y_train, epochs=num_epocs, batch_size=run_batch_size, validation_split=0.3, callbacks=[early_stopping])


predictions = model.predict(X_test)
#ShowImportances(data.columns[:input_features],model.feature_importances_, False)
#show_stats(False, y_test, predictions)

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

