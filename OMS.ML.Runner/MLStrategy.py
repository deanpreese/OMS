from enum import Enum
import json
import mlflow
import pandas as pd
import requests
import datetime as dt
import random as rand
from models import wrapped_models

from CommonStrategy import position_status, order_action, CommonStrategy

class MLStrategy (CommonStrategy):
    def __init__(self, model, col_filter, run_id):
        super().__init__()
        
        self.model = model
        self.column_filter = col_filter
        self.run_id = run_id
        self.run_name = ""

        self.trader_id = 0
        self.trader_group = 55
        

        
        
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
        
  