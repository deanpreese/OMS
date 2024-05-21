import json
import mlflow
import pandas as pd
import requests
import datetime as dt
import random as rand


def convert_model(self, rid, isReg):
        rinfo = mlflow.get_run(rid)
        run_txt = f"runs:/{rid}/model" 
        
        loaded_model = mlflow.pyfunc.load_model(run_txt)
        

def add_runs(experiment_id):
    runs = mlflow.search_runs(experiment_id)
    for index, run in runs.iterrows():    
        run_id = run.run_id
        convert_model(run_id, True)


            
            
            