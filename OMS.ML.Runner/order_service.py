import csv
import datetime
from flask import Flask, Response, request, jsonify
import numpy as np
import pandas as pd
from io import StringIO
import cProfile

from ModelLoader import ModelLoader
from OrderManager import OrderManager

order_manager = OrderManager()
model_loader = ModelLoader()
models = []

import logging
logging.getLogger('mlflow.utils.autologging_utils').setLevel(logging.ERROR)

def LoadModels():
    m = []
    #experiment_id = ["792022387336146046"]
    #experiment_id = ["748668048429049790"]
    
    experiment_id = ["1"]
    m = model_loader.load_random_models(experiment_id, 5)
    return m

def init_app():
    app = Flask(__name__)

    with app.app_context():
        models = LoadModels()
       
    @app.route('/predict', methods=['POST'])
    def predict():
        
        csv_data = StringIO(request.data.decode('utf-8'))
        column_names = ['time', 'SDLR310', 'SDBB91', 'SDKC91', 'SDKC9', 'ROC', 'ATR34', 'ATR32', 'ATR31', 'ATR3', 'ATR21', 'ATR2', 'RSI', 'STOK1', 'output', 'outputC', 'actual']
        data_df = pd.read_csv(csv_data, header=None, names=column_names)

        time = data_df['time']
        px = data_df['actual']
        data_df.drop(columns=['time', 'actual', 'output', 'outputC'], inplace=True)
        
        for m in range(len(models)):
            loaded_prediction = models[m].do_predict(data_df)
            order_manager.process_model(models[m], px[0], loaded_prediction[0])
            
        order_manager.process_tick_rt(px[0],time[0])
        
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
    