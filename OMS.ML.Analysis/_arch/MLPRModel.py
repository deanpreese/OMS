import pandas as pd
import numpy as np
import pickle
from itertools import product
from sklearn.discriminant_analysis import StandardScaler
from sklearn.model_selection import train_test_split
from sklearn.neural_network import MLPRegressor
from sklearn.metrics import mean_squared_error
from sklearn.preprocessing import MinMaxScaler, StandardScaler
import matplotlib.pyplot as plt
from common_func import gen_importances, show_stats

import warnings
warnings.filterwarnings("ignore")

from models.BaseModel import BaseModel

class MLPRModel(BaseModel):

    def __init__(self, input_features):
        
        self.input_features = input_features

        layers =  [32,128,128,32]
    
        params = { 'hidden_layer_sizes' : layers ,
            #'activation' : 'relu',
            #'solver' : 'adam',
            #'alpha' : 0.0,
            'batch_size' : 16,
            'random_state' : 0,
            #'tol' : 0.00001,
            #'nesterovs_momentum' : True,
            #'learning_rate' : 'invscaling',
            #'learning_rate' : 'constant',
            #'learning_rate_init' : 0.001,
            #'learning_rate_init' : 0.00088,
            'max_iter' : 1000,
            #'shuffle' : False,
            'early_stopping' : True,
            'n_iter_no_change' : 25,
            'verbose' : True }


        super().__init__(MLPRegressor, params)
        
        
    def ParamGrid(self):
        
            first_layer_neurons = np.arange(3, 100, 3)
            second_layer_neurons = np.arange(5, 100, 5)
            third_layer_neurons = np.arange(3, 20, 1)
            hidden_layer_sizes = list(product(first_layer_neurons, second_layer_neurons,third_layer_neurons))
            #hidden_layer_sizes = list(product(first_layer_neurons, second_layer_neurons))
            #hidden_layer_sizes = list(product(first_layer_neurons))
        
            grid = {
               'hidden_layer_sizes': hidden_layer_sizes,
                'batch_size' : [16],
                'random_state' : [0],
                #'tol' : 0.00001,
                #'nesterovs_momentum' : True,
                #'learning_rate' : 'invscaling',
                #'learning_rate' : 'constant',
                #'learning_rate_init' : 0.001,
                #'learning_rate_init' : 0.00088,
                'max_iter' : [1000],
                #'shuffle' : False,
                'early_stopping' : [True],
                'n_iter_no_change' : [25],
                'verbose' : [True] 
            }    
        
            return grid

        
        