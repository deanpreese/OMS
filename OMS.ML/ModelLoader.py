import json
import mlflow
import pandas as pd
import requests
import datetime as dt
import random as rand
from MLStrategy import MLStrategy
from models import wrapped_models

class ModelLoader:

    def __init__(self):
        self.experiment_id = 0
        self.l_models = []
        self.l_artifacts = []
        self.model_list = []

    def add_model(self, rid):
        rinfo = mlflow.get_run(rid)
        run_txt = f"runs:/{rid}/model" 
        
        print(run_txt)
        
        
        loaded_model = mlflow.pyfunc.load_model(run_txt)
        
        
        
        cols =[]
        try:
            art = json.loads(rinfo.data.tags['mlflow.loggedArtifacts'])
            art_file = art[0].get('path', None)
            art_uri = rinfo.info.artifact_uri
            art_to_load = f"{art_uri}/{art_file}"
            print(art_to_load)
            arti_d = mlflow.artifacts.load_dict(art_to_load)
            cols = [x[0] for x in arti_d['data'] if x[0] != 'output']
            self.l_artifacts.append(cols)
        except Exception as e:
            print(f"An error occurred: {e}")
            cols = []    
        
        self.l_models.append(loaded_model)
        
        #lm = LoadedModel(loaded_model, cols,rid)
        lm = MLStrategy(loaded_model, cols,rid)
        
        lm.run_name = rinfo.info.run_name
        
        self.initialize_trader(lm)
                
        self.model_list.append(lm)        
                
        return loaded_model, cols

    def load_selected_models(self, runs):        
        for index, run in runs.iterrows():    
            run_id = run.run_id
            self.add_model(run_id)
            
        return self.model_list

    def load_models(self, experiment_id):
        runs = mlflow.search_runs(experiment_id)
        self.load_selected_models(runs)
        
        return self.model_list    
            
    def load_random_models(self, experiment_id, num_models): 
        
        print("Loading Runs ...")
        #runs = mlflow.search_runs(experiment_ids=experiment_id, filter_string="", order_by=["metrics.MSE DESC"], max_results=num_models)
        runs = mlflow.search_runs(experiment_id)
        idxx = rand.sample(range(1, len(runs) -2 ), num_models)
        
        for i in idxx:
            r_id = runs.iloc[i].run_id 
            print(f"Run Id     {r_id}")
            self.add_model(r_id)
        
        return self.model_list     

    def initialize_trader(self, lm):
        
        url = "http://localhost:8786/api/ml/verify-model-trader"  # Replace with the actual URL of the web service

        headers = {
            "Content-Type": "application/json"
        }

        add_trader_model = {  
            "group": 55,
            "userId": 0,
            "displayName": lm.run_name,
            "userPwd": "abc",
            "firstName": "Model",
            "lastName": "Trader",
            "email": "bac.abc"
        }
                        
        response = requests.post(url, data=json.dumps(add_trader_model), headers=headers)

        if response.status_code == 200:
            result = response.json()
            lm.trader_id = result
            
        else:
            # Error handling
            print(f"Trader initialize request failed with status code {response.status_code}")
                    