from flask import Flask, request, jsonify
import numpy as np
from sklearn.ensemble import GradientBoostingRegressor
from tensorflow import keras
import pickle

import pandas as pd
from model_manager import ModelLoader
import models.wrapped_models

app = Flask(__name__)

runner = ModelLoader()

experiment_id = ["992846708356537991"]
#models = runner.load_models(experiment_id)
models = runner.load_random_models(experiment_id, 10)

        
@app.route('/predict', methods=['POST'])
def predict():
    try:
        # Receive input data as JSON
        input_data = request.get_json()

        print(input_data)

        j_rtn = {"error": "Missing input features"}
        
        """
        j_rtn = {
            "prediction": str(all_predictions),
            "lgb": str(lgb_prediction),
            "xgb": str(xgb_prediction),
            "confidence": str(confidence)
            }
        """ 

        return jsonify(j_rtn)

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, port=8000, host='0.0.0.0')