import itertools
import pandas as pd
import xgboost as xgb
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score

import datetime as dte_time
import random as rand
import uuid
#import warnings
import mlflow

import pandas as pd

from models.wrapped_models import TunableCatBoostRegressor, TunableLGBMRegressor, TunableXGBRegressor
from common.common_func import calc_MSE, calc_reg_results, calc_reg_streaks, show_stats, simple_split_and_scale


time_stamp = dte_time.datetime.utcnow().strftime('%Y%m%d%H%M%S%f')
exp_name = f"magic_13_{time_stamp}"

try:
    experiment_id = mlflow.create_experiment(exp_name)
except Exception as e:
    print(f"{e}")    
    experiment_id = mlflow.get_experiment_by_name(exp_name).experiment


#train_file = pd.read_csv('data/sm13_3070.csv')
train_file = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
data_loaded = train_file.drop(columns=['outputC'])
features = data_loaded.columns[:-1]
target = 'output'

# Generate all combinations of 13 features
feature_combinations = itertools.combinations(features, 2)

results = {}
random_state = 42
comb = 0

for i in range(2,len(features)):

    feature_combinations = itertools.combinations(features, i)

    for combination in feature_combinations:
        X = data_loaded[list(combination)]
        y = data_loaded[target]
        
        comb += 1

        print(" ")        
        print(f"Combination: {combination}  " )
        print(" ")        
        
        X_train, X_test, y_train, y_test = simple_split_and_scale(X, y, 0.7, 42)
        
        e = TunableXGBRegressor()
        e.track_model(exp_name, True, e, X_train, y_train, X_test, y_test)  
        

print(f"Total combinations: {comb}")    

