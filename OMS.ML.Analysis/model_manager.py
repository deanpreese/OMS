import json
import mlflow
import pandas as pd


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


    def process_models(self, data):
        
         for m in range(len(self.model_list)):
            
            pass 
            #loaded_prediction = self.model_list[m].do_predict( data )
            
            