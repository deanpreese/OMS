import csv
import datetime
from flask import Flask, Response, request, jsonify
import numpy as np
import pandas as pd
from io import StringIO
import cProfile
from io import BytesIO
import time as mytime


from ModelLoader import ModelLoader
from OrderManager import OrderManager

order_manager = OrderManager()
order_manager.is_sim(False)

model_loader = ModelLoader()
models = []

import logging
logging.getLogger('mlflow.utils.autologging_utils').setLevel(logging.ERROR)
logging.getLogger('mlflow.pyfunc').setLevel(logging.ERROR)



def LoadModels():
    m = []
   
    experiment_id = ["50"]
    num_models = 1    

    # USed for base models
    #m = model_loader.load_random_models(experiment_id, 5)
    
    #used for comp models
    m = model_loader.load_composite_models( experiment_id, num_models)
    
    
    return m

def init_app():
    app = Flask(__name__)

    with app.app_context():
        models = LoadModels()
       
       
    @app.route('/predict', methods=['POST'])
    def predictx():
        
        csv_data = BytesIO(request.data)
        column_names = ['time', 'SDLR310', 'SDBB91', 'SDKC91', 'SDKC9', 'ROC', 'ATR34', 'ATR32', 'ATR31', 'ATR3', 'ATR21', 'ATR2', 'RSI', 'STOK1', 'output', 'outputC', 'actual']
        data_df = pd.read_csv(csv_data, header=None, names=column_names)
        time = data_df["time"]
        px = data_df["actual"]
        data_df.drop(columns=['time', 'actual', 'output', 'outputC'], inplace=True)
        
        print(csv_data)
        
        """
        for m in range(len(models)):
            loaded_prediction = models[m].do_predict(data_df)
            
            print(f"Model-{m}    {loaded_prediction}")
            
            # used for base models
            #order_manager.process_model(models[m], px[0], loaded_prediction[0])
            
            #used for comp models
            order_manager.process_model(models[m], px[0], loaded_prediction)
            
            mytime.sleep(0.15)
            
            
        order_manager.process_tick_rt(px[0],time[0])
        """
        
        
        return "ok"       
        
    @app.route('/close-all-open', methods=['POST'])
    def close_all_open():
        for m in range(len(models)):
            order_manager.close_all(models[m])
    
        return "ok"
    
    
    @app.route('/reload-models', methods=['POST'])
    def reload_models():
        models = LoadModels()
        for m in range(len(models)):
            m.position = 0
            
        return "ok"
    
        
    return app

   

app = init_app()
    
if __name__ == '__main__':
    print("Starting Flask application.")
      
    
    app.run(
        debug=True, 
        use_reloader=False,
        port=8888, 
        host='0.0.0.0'
        )
    