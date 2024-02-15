import pandas as pd
import numpy as np
import pickle
import matplotlib.pyplot as plt
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from sklearn.preprocessing import StandardScaler
from keras.models import Sequential
from keras.layers import Dense, LSTM, Dropout, Input, Embedding, Bidirectional,TimeDistributed
from sklearn.ensemble import RandomForestRegressor
from keras.callbacks import EarlyStopping

early_stopping = EarlyStopping(monitor='loss',patience=5)
from common_func import gen_importances, show_stats, calc_MSE
from sklearn.metrics import mean_squared_error


class MLPRTFModel:
    def __init__(self, input_features):
        self.num_epocs = 100
        self.run_batch_size = 32
        self.run_test_size = 0.5
        self.input_features = input_features
        self.load_model()
        print("MLPR TF Model")

    def load_model_data_from_file(self, file_to_load):
        data = pd.read_csv(file_to_load)
        num_columns = len(data.columns)
        self.input_features = num_columns - 1
        X = data.iloc[:, 0:self.input_features]
        y = data['output'].values

        self.X_train, self.X_test, self.y_train, self.y_test = train_test_split(X, y, test_size=self.run_test_size,
                                                                                random_state=0)

    def load_model_data(self, input_features, X_train, y_train, X_test, y_test):
        self.X_train = X_train
        self.X_test = X_test
        self.y_train = y_train
        self.y_test = y_test
        self.input_features = input_features


    def load_model_using_params(self,params):
        print("MLPR TF Model Does not load from params ")
        print(self.model)
        print(" ")

    def load_model(self):
      
        hlayer0 = int(self.input_features * .7)
        hlayer1 = int(self.input_features * .65)
        hlayer2 = int(self.input_features * .50)
        hlayer3 = int(self.input_features * .35)
        hlayer4 = int(self.input_features * .25)

        
        model = Sequential()
        model.add(Input(shape = (self.input_features,),))
        model.add(Dense(hlayer2,activation='relu'))
        #model.add(BatchNormalization())
        #model.add(Dropout(0.5))
        #model.add(Dense(hlayer2,activation='relu'))
        model.add(Dense(hlayer4,activation='relu'))
        #model.add(Dropout(0.2))
        #model.add(Dense(hlayer4,activation='relu'))
        #model.add(BatchNormalization())
        model.add(Dense(1, activation='linear'))
        #model.add(Dense(1))
       
        self.model = model

        model.compile(loss='mean_squared_error', optimizer='adam', 
              metrics=['F1Score','RootMeanSquaredError', ])
        
        print(" ")
        print(f"{self.model.summary()}")
        print(" ")

        

    def rescale_data(self, scaler):
        self.X_train = scaler.fit_transform(self.X_train)
        self.X_test = scaler.transform(self.X_test)
   

    def scale_data(self):
        scaler = StandardScaler()
        self.X_train = scaler.fit_transform(self.X_train)
        self.X_test = scaler.transform(self.X_test)

    def show_data_shapes(self):
        print("Number of TRAIN patterns:", self.X_train.shape[0])
        print("Size of input patterns:", self.X_train.shape[1])
        print(" ")
        print("Number of TEST patterns:", self.X_test.shape[0])
        print("Size of input patterns:", self.X_test.shape[1])
        print(" ")

    def train_model(self):
        early_stopping = EarlyStopping(monitor='loss',patience=5)
        #history = self.model.fit(self.X_train, self.y_train, epochs=self.num_epocs, batch_size=self.run_batch_size, validation_split=self.run_test_size, callbacks=[early_stopping],)
        self.model.fit(self.X_train, self.y_train, epochs=self.num_epocs, batch_size=self.run_batch_size, validation_split=self.run_test_size, callbacks=[early_stopping],)


    def predict_model(self):
        return self.model.predict(self.X_test)
    
    def stats(self, display):
        self.predicted_values = self.model.predict(self.X_test)
        show_stats(display, self.y_test, self.predicted_values)

    def MSE_RMSE(self, display):
        return calc_MSE(self.y_test, self.predicted_values, True)

    def save_model(self, model_filename):
        print(" ")
        print("Saving Model ")
        pickle.dump(self.model, open(model_filename, "wb"))

    def load_saved_model(self, model_filename):
        print(" ")
        print("Loading Model ")
        self.model = pickle.load(open(model_filename, "rb"))
