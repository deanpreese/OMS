import json
import mlflow
import pandas as pd
import requests
import datetime as dt
import random as rand
import time


from models import wrapped_models

from ModelLoader import ModelLoader
from SimOrderManager import SimOrderManager

from  common.CommonCli import CommonCli as common_cli

from CompositeStrategy import CompositeStrategy    
import logging
logging.getLogger('mlflow.utils.autologging_utils').setLevel(logging.ERROR)


file = "data/lucky13_oos.csv"
data = pd.read_csv(file)   
num_columns = len(data.axes[1]) 
input_features =  num_columns -2
X = data.iloc[:, 0:input_features]  
y = data["output"].values

experiment_id = ["6"]
num_models = 1    
model_loader = ModelLoader()
models = model_loader.load_composite_models( experiment_id, num_models)

order_manager = SimOrderManager()

for item in models: 
    
    print(" ")
    print(item.run_name)
    print(item.run_id)
    
    for r in item.strategy_models:
        print(r.model)
        print(r.metrics["Perf"])
        print(" ")



total = 0
order_total = 0
start = time.time()

win = 0
loss = 0
skip_signal = 0

for i in range(len(y)):

        for m in range(len(models)):
            
            predict = comp_prediction = models[m].do_predict(X.iloc[i])
            
            if predict == 0:
                print(f" <---> {models[m].comp_run_name}  {y[i]}   {predict}  " )
                skip_signal += 1          
            
            elif  (predict > 0  and y[i] > 0  ) :
                print(f" +++ {models[m].comp_run_name}   {y[i]}   {predict}    " )
                win += 1
            
            elif  (predict < 0  and y[i] < 0  ) :
                print(f" +++ {models[m].comp_run_name} {y[i]}  {predict} " )
                win+=1
                
            elif  ( (predict < 0 and y[i] > 0)   
                    or (predict > 0 and y[i] < 0)  ):
                        print(f" --- {models[m].comp_run_name}  {y[i]}   {predict}  " )
                        loss += 1
                                 
            order_manager.process_model(models[m], y[i], predict)            
            order_total += 1

        #print(f"Order Count {total}")
                
        total += 1        
        order_manager.process_tick(y[i])
       
        if total > 500:
            break    


for m in range(len(models)):
    order_manager.close_all(models[m])


end = time.time()

print(f"Win {win} ")
print(f"Loss {loss} ")
print(f"Skipped {skip_signal} ")

print(f"Total {win/(win+loss)} ")


#for m in range(len(models)):
#    print(f" {models[m].trader_id}  {models[m].run_name}  {models[m].run_id}   {models[m].win_cnt}   {models[m].loss_cnt}    {models[m].win_cnt / ((models[m].win_cnt + models[m].loss_cnt))}      {models[m].winners}   {models[m].losses}      {models[m].winners - models[m].losses}  " )

t = round(end-start,2)
print(f"Time {t} seconds to process {order_total} predictions  --  {round(order_total/t,2)}/sec ")
print(" ") 