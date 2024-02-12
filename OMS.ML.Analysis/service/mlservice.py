from flask import Flask, request, jsonify
import numpy as np
from sklearn.ensemble import GradientBoostingRegressor
from tensorflow import keras
import pickle

app = Flask(__name__)


# Load the trained model (replace with your model loading code)
xgb_filename = "XGBR.pkl"
xgb_model = pickle.load(open(xgb_filename, "rb"))
xgb_scaler_filename = "XGB_Scaler.pkl"        
xgb_loaded_scaler = pickle.load(open(xgb_scaler_filename, "rb"))

lgbm_filename = "LGBM.pkl"
lgb_model = pickle.load(open(lgbm_filename, "rb"))
lgb_scaler_filename = "LGBM_Scaler.pkl"        
lgb_loaded_scaler = pickle.load(open(lgb_scaler_filename, "rb"))


#XGB 0.3944634103980186      7.608923300504618      2.758427686292432           0.7547
#LGB  0.3880083643333536       7.6900347498384125       2.7730911903214457            0.7558
xgb_perf = .7547
xgb_score = .3945
lgb_perf = .7558
lgb_score = .3880

# weights taken from training data work
#weights = [lgb_perf, xgb_perf]  
weights = [lgb_score, xgb_score]  

        
@app.route('/predict', methods=['POST'])
def predict():
    try:
        # Receive input data as JSON
        input_data = request.get_json()

        print(input_data)

        # Check if the input contains the expected features
        if ('feature1' not in input_data or 'feature2' not in input_data or 
            'feature3' not in input_data or 'feature4' not in input_data or 
            'feature5' not in input_data or 'feature6' not in input_data or 
            'feature7' not in input_data or 'feature8' not in input_data or 
            'feature9' not in input_data or 'feature10' not in input_data or 
            'feature11' not in input_data or 'feature12' not in input_data or 
            'feature13' not in input_data):
           
            return jsonify({"error": "Missing input features"}), 400

        # Extract input features
        feature1 = input_data['feature1']
        feature2 = input_data['feature2']
        feature3 = input_data['feature3']
        feature4 = input_data['feature4']
        feature5 = input_data['feature5']
        feature6 = input_data['feature6']
        feature7 = input_data['feature7']
        feature8 = input_data['feature8']
        feature9 = input_data['feature9']
        feature10 = input_data['feature10']
        feature11 = input_data['feature11']
        feature12 = input_data['feature12']
        feature13 = input_data['feature13']

        # Prepare input features for prediction
        input_features = np.array([[feature1, feature2, feature3, feature4, 
                                    feature5, feature6, feature7,feature8, feature9, feature10, 
                                    feature11, feature12, feature13]])

        xgb_data = xgb_loaded_scaler.transform(input_features)    
        xgb_prediction = xgb_model.predict(xgb_data)[0]

        lgb_test = lgb_loaded_scaler.transform(input_features)    
        lgb_prediction = lgb_model.predict(lgb_test)[0]


        all_predictions = (lgb_prediction + xgb_prediction)/2

        # Combine predictions using weighted average
        combined_prob = (lgb_prediction * weights[0] +                
                        xgb_prediction * weights[1]                                 
                        ) / sum(weights)

        confidence = 0
        
        if all_predictions > 0:
            confidence = 0.5
            
        if all_predictions < 0:            
            confidence = 0.5
        
        if all_predictions > 0.5:
            confidence = 1
            
        if all_predictions < -0.5:            
            confidence = 1
        
        #if ((combined_prob * all_predictions)  > 0) :
        #    confidence = 2
        
        #if ((combined_prob * all_predictions)  > 0) :
        #    confidence = 2
        
        print(f" comb prob {combined_prob}  tot_pred {all_predictions}")
        print(f"lgb {lgb_prediction}  xgb {xgb_prediction}  confidence {confidence} "  )

        j_rtn = {
            "prediction": str(all_predictions),
            "lgb": str(lgb_prediction),
            "xgb": str(xgb_prediction),
            "confidence": str(confidence)
            }


        return jsonify(j_rtn)

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, port=8000, host='0.0.0.0')