import json
import mlflow
import pandas as pd
import requests
import datetime as dt
import random as rand
import time

import logging
logging.getLogger('mlflow.utils.autologging_utils').setLevel(logging.ERROR)
logging.getLogger('mlflow.pyfunc').setLevel(logging.ERROR)

from models import wrapped_models
from ModelLoader import ModelLoader
from OrderManager import OrderManager

from  common.CommonCli import CommonCli as common_cli
from CompositeStrategy import CompositeStrategy    

def load_models(exp_id, n_models):
    
    experiment_id = exp_id
    num_models = n_models    
    model_loader = ModelLoader()
    return model_loader.load_composite_models( experiment_id, num_models)

def run_sim(exp_id, n_models, file, trades, delay):


    data = pd.read_csv(file)   
    num_columns = len(data.axes[1]) 
    input_features =  num_columns -2
    X = data.iloc[:, 0:input_features]  
    y = data["output"].values
    
    models = load_models(exp_id, n_models)

    order_manager = OrderManager()
    order_manager.is_sim(True)
    
    
    total = 0
    order_total = 0
    start = time.time()

    for i in range(len(y)):
           

            for m in range(len(models)):
                
                predict = models[m].do_predict(X.iloc[i])
                order_manager.process_model(models[m], y[i], predict)            
                order_total += 1
                
                time.sleep(delay)

            total += 1        
            order_manager.process_tick(y[i])

            if total > trades:
                break    


    for m in range(len(models)):
        order_manager.close_all(models[m])

    end = time.time()

    t = round(end-start,2)
    print(f"Time {t} seconds to process {order_total} predictions  --  {round(order_total/t,2)}/sec ")
    print(" ") 
    
    
exp_idx = ["2"]
num_models = 10
trades = 250
sim_delay = 0.000005

#file = "data/lucky13_short.csv"
    
file = "data/lucky13_oos.csv"    
    
run_sim(exp_idx, num_models, file, trades, sim_delay)    

