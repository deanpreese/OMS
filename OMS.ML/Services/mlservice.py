from flask import Flask, request, jsonify
import json
import pandas as pd

app = Flask(__name__)



        
@app.route('/predict', methods=['POST'])
def predict():
    try:
      

        return jsonify(0)

    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True, port=8000, host='0.0.0.0')