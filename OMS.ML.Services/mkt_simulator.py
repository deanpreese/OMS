import pandas as pd

import ModelLoader

import models.wrapped_models
import order_manager

from time import sleep
import time

#from models.WrappedModels import *  
#from CommonFunc import *
#from model_manager import *
#from order_manager import *


file = "data/lucky13_oos.csv"
data = pd.read_csv(file)   
num_columns = len(data.axes[1]) 
input_features =  num_columns -2
X = data.iloc[:, 0:input_features]  
y = data["output"].values

order_manager = order_manager.SimOrderManager()
runner = ModelLoader()

experiment_id = ["792022387336146046"]
#models = runner.load_models(experiment_id)
models = runner.load_random_models(experiment_id, 5)

total = 0

order_total = 0

start = time.time()

for i in range(len(y)):

        for m in range(len(models)):
            
            sleep(1)
            
            loaded_prediction = models[m].do_predict(X.iloc[i])
            order_manager.process_order(models[m], y[i], loaded_prediction)
            order_total += 1

        print(f"Order Count {total}")
                
        total += 1        
        order_manager.process_tick(y[i])
       
        if total > 10:
            break    

end = time.time()

print(" ")

for m in range(len(models)):
    print(f" {models[m].trader_id}  {models[m].run_name}  {models[m].run_id}   {models[m].win_cnt}   {models[m].loss_cnt}    {models[m].win_cnt / ((models[m].win_cnt + models[m].loss_cnt))}      {models[m].winners}   {models[m].losses}      {models[m].winners - models[m].losses}  " )
 
print(" ") 
print(f"Time {end-start} seconds to process {order_total}")