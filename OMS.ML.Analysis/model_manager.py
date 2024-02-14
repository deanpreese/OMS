import json
import mlflow
import pandas as pd
import requests
import json
import datetime as dt

class LoadedModel:
    def __init__(self, model, col_filter, run_id):
        self.model = model
        self.column_filter = col_filter
        self.run_id = run_id
        
        self.winners = 0
        self.losses = 0
        self.win_cnt = 0
        self.loss_cnt = 0
        
        self.run_name = ""
        self.trader_id = 0
        self.last_prediction = 0
        
    def do_predict(self,data):

        if len(self.column_filter) == 0:
            XD = data
            self.column_filter = list(data.keys())
        else:
            XD = data[self.column_filter]
            
        d = XD.to_numpy().reshape(1,-1)
        df = pd.DataFrame(d)
        df.columns = self.column_filter
        
        return self.model.predict(df)
        

    def check_for_orders(self, prediction, px):
        
        order_action = "Buy"
        
        orders = []
        
        if self.last_prediction == 0:
            self.last_prediction = prediction
            
            if prediction < 0 :    
                order_action = "Sell"
            
            # build new order
            orders.append(self.build_order( order_action,px))
            
           
        elif self.last_prediction < 0 and prediction > 0 or self.last_prediction > 0 and prediction < 0 :
            
            o_t_c = "Buy"
            if self.last_prediction < 0:
                o_t_c = "Sell"
            
            # build closing order for last    
            orders.append(self.build_order( o_t_c,px))    
                        
            o_n = "Buy"
            if prediction < 0:
                o_n = "Sell" 
                                       
            # build new order with new prediction
            orders.append(self.build_order(o_n,px))                          
            
            self.last_prediction = prediction
            
        elif self.last_prediction > 0 and prediction > 0 or self.last_prediction > 0 and prediction > 0 : 
            # ignore order
            self.last_prediction = prediction
    
        return orders
    
    
    def build_order(self, order_action, px):
        
        dt_string = dt.datetime.utcnow()
        dte_iso = dt_string.isoformat()
        
        new_order = {
            "newOrderID": 0,
            "platformOrderID": 0,
            "userName": self.run_name,
            "userID": self.trader_id,
            "userGroup": 55,
            "authToken": 0,
            "instrument": "ES",
            "orderPX": px,
            "orderType": "Market",
            "orderAction": order_action,
            "quantity": 1,
            "orderTime": dte_iso,
        }    
    
        return new_order

        

class ModelLoader:

    def __init__(self):
        self.experiment_id = 0
        self.l_models = []
        self.l_artifacts = []
        self.model_list = []

    def add_model(self, rid):
        rinfo = mlflow.get_run(rid)
        run_txt = f"runs:/{rid}/model" 
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
        lm = LoadedModel(loaded_model, cols,rid)
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
        
        runs = mlflow.search_runs(experiment_ids=experiment_id, filter_string="", order_by=["metrics.MSE DESC"], max_results=num_models)
        self.load_selected_models(runs)
        
        return self.model_list

           

    def initialize_trader(self, lm):
        
        url = "http://localhost:8786/api/order/verify-model-trader"  # Replace with the actual URL of the web service

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
                    