import pandas as pd
import numpy as np
import pickle
from sklearn.discriminant_analysis import StandardScaler
from sklearn.model_selection import train_test_split
from sklearn.neural_network import MLPRegressor
from sklearn.metrics import mean_squared_error
from sklearn.preprocessing import MinMaxScaler, StandardScaler
import matplotlib.pyplot as plt
from common_func import ShowImportances, show_stats

import warnings
warnings.filterwarnings("ignore")

max_runs = 100
run_batch_size = 32
run_test_size = 0.7

model_filename = "saved_models/mlpr.ml"


#data = pd.read_csv("data/Seq_30_3070_TAG_X2.csv")   # 75% 
#data = pd.read_csv("data/Seq_30_R2080_TAG.csv")   #83%
#data = pd.read_csv("data/Seq_30_R3070_TAG.csv")   #75%   
#data = pd.read_csv("data/Seq_30_2080_TAG_X2.csv")   #  63%

#data = pd.read_csv("data/Seq_TopSet_R3070_TAG.csv")   #  
data = pd.read_csv("data/Seq30_R3070_TAG.csv")   #  

num_columns = len(data.axes[1]) 
input_features =  num_columns -1
X = data.iloc[:, 0:input_features]  
y = data['output'].values


X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=run_test_size, random_state=0, shuffle=False)

print("Number of TRAIN patterns:", X_train.shape[0])
print("Size of input patterns:", X_train.shape[1])
print(" ")
print("Number of TEST patterns:", X_test.shape[0])
print("Size of input patterns:", X_test.shape[1])
print(" ")

# Normalize the data
#scaler = StandardScaler()
scaler = MinMaxScaler(feature_range=(-1, 1))
X_train = scaler.fit_transform(X_train)
X_test = scaler.transform(X_test)


params5 = { 'hidden_layer_sizes' :  [36,72,2,1],
    'activation' : 'relu',
    'solver' : 'adam',
    #'alpha' : 0.0,
    'batch_size' : 32,
    'random_state' : 0,
    #'tol' : 0.000001,
    #'nesterovs_momentum' : False,
    #'learning_rate' : 'invscaling',
    #'learning_rate' : 'constant',
    'learning_rate_init' : 0.001,
    #'learning_rate_init' : 0.00088,
    'max_iter' : 250,
    #'shuffle' : False,
    'early_stopping' : True,
    'n_iter_no_change' : 10,
    'verbose' : True }

params6 = { 'hidden_layer_sizes' :  [75,50,2,1],
    #'activation' : 'relu',
    #'solver' : 'adam',
    #'alpha' : 0.0,
    #'batch_size' : 32,
    'random_state' : 0,
    #'tol' : 0.000001,
    #'nesterovs_momentum' : False,
    #'learning_rate' : 'invscaling',
    #'learning_rate' : 'constant',
    #'learning_rate_init' : 0.001,
    'learning_rate_init' : 0.00088,
    'max_iter' : 250,
    #'shuffle' : False,
    #'early_stopping' : True,
    'n_iter_no_change' : 25,
    'verbose' : True }


hlayer0 = int(input_features * .9)
hlayer1 = int(input_features * .75)
hlayer2 = int(input_features * .66)
hlayer3 = int(input_features * .5)
hlayer4 = int(input_features * .25)

params7 = { 'hidden_layer_sizes' :  [hlayer1,hlayer3,2,1],
    #'activation' : 'relu',
    #'solver' : 'adam',
    #'alpha' : 0.0,
    #'batch_size' : 32,
    'random_state' : 0,
    #'tol' : 0.000001,
    #'nesterovs_momentum' : False,
    #'learning_rate' : 'invscaling',
    #'learning_rate' : 'constant',
    #'learning_rate_init' : 0.001,
    'learning_rate_init' : 0.00088,
    'max_iter' : 250,
    #'shuffle' : False,
    #'early_stopping' : True,
    'n_iter_no_change' : 25,
    'verbose' : True }


model = MLPRegressor(**params7)

print(" ")
print(f'{model}')
print(" ")

model.fit(X_train, y_train)

print(" ")
print("Saving and Reloading Model ")
pickle.dump(model, open(model_filename, "wb"))
loaded_model = pickle.load(open(model_filename, "rb"))
predicted_values = loaded_model.predict(X_test)


predictions = model.predict(X_test)
#ShowImportances(data.columns[:input_features],model.feature_importances_, False)
show_stats(False, y_test, predictions)

