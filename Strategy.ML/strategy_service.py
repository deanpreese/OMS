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
model_loader = ModelLoader()
models = []

import logging
logging.getLogger('mlflow.utils.autologging_utils').setLevel(logging.ERROR)
logging.getLogger('mlflow.pyfunc').setLevel(logging.ERROR)


def LoadModels():
    m = []
   
    
    experiment_id = ["25"]
    num_models = 1    

    # USed for base models
    #m = model_loader.load_random_models(experiment_id, 5)
    
    #used for comp models
    m = model_loader.load_composite_models( experiment_id, num_models)
    
    return m

def init_app():
    app = Flask(__name__)

    with app.app_context():
        #pass        
        models = LoadModels()
       
       
    @app.route('/predict', methods=['POST'])
    def predictx():
        
        csv_data = BytesIO(request.data)
        column_names = ['time', 'SDLR310', 'SDBB91', 'SDKC91', 'SDKC9', 'ROC', 'ATR34', 'ATR32', 'ATR31', 'ATR3', 'ATR21', 'ATR2', 'RSI', 'STOK1', 'output', 'outputC', 'actual']
        data_df = pd.read_csv(csv_data, header=None, names=column_names)
        
        #time_raw = data_df["time"]
        #px_f = float(data_df["actual"][0])
        data_df.drop(columns=['time', 'actual', 'output', 'outputC'], inplace=True)
        #print( data_df )
       
        loaded_prediction = 0
       
        for m in range(len(models)):
            loaded_prediction = models[m].do_predict(data_df)
            print(f"Model-{m}    {loaded_prediction}")
            
        return str(loaded_prediction)
        
    return app

   

app = init_app()
    
if __name__ == '__main__':
    print("Starting Flask application.")
      
    
    app.run(
        debug=True, 
        use_reloader=False,
        port=9999, 
        host='0.0.0.0'
        )
    