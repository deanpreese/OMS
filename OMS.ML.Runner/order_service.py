import csv
import datetime
from flask import Flask, Response, request, jsonify
import numpy as np
import pandas as pd
from io import StringIO

from ModelLoader import ModelLoader
from OrderManager import OrderManager

order_manager = OrderManager()
model_loader = ModelLoader()
models = []

import logging
logging.getLogger('mlflow.utils.autologging_utils').setLevel(logging.ERROR)


def ProcessModels(data, px):
    for m in range(len(models)):
            loaded_prediction = models[m].do_predict(data)
            order_manager.process_model(models[m], px, loaded_prediction)

def init_app():
    app = Flask(__name__)

    with app.app_context():
        #experiment_id = ["792022387336146046"]

        experiment_id = ["249686191457248322"]
        models = model_loader.load_random_models(experiment_id, 3)             

        
    @app.route('/predict', methods=['POST'])
    def predict():
        
        # Convert the request data to a StringIO object, then read it into a DataFrame
        csv_data = StringIO(request.data.decode('utf-8'))
        data_df = pd.read_csv(csv_data,header=None)
        data_df.columns = ['time','SDLR310','SDBB91','SDKC91','SDKC9','ROC','ATR34','ATR32','ATR31','ATR3','ATR21','ATR2','RSI','STOK1','output','outputC','actual']
        time = data_df['time']
        px = data_df['actual']
        
        data_xx = data_df
        
        data_xx.drop('time', axis=1, inplace=True)
        data_xx.drop('actual', axis=1, inplace=True)
        data_xx.drop('output', axis=1, inplace=True)
        data_xx.drop('outputC', axis=1, inplace=True)
        
        
        for m in range(len(models)):
            loaded_prediction = models[m].do_predict(data_xx)
            #print(f"XYZ    {loaded_prediction[0]}  {time[0]}  {px[0]}")                
            
            order_manager.process_model(models[m], px[0], loaded_prediction[0])
            order_manager.process_tick_rt(px[0],time[0])
        
        return Response("CSV data processed successfully", status=200)        
        
    @app.route('/close-all-open', methods=['POST'])
    def close_all_open():
        for m in range(len(models)):
            order_manager.close_all(models[m])

        
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
