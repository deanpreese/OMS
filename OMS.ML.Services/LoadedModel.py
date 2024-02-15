import json
import mlflow
import pandas as pd
import requests
import datetime as dt
import random as rand



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
            if prediction < 0 :    
                order_action = "Sell"
            
            # build new order
            orders.append(self.build_order( order_action,px))
            self.last_prediction = prediction
           
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
            
        elif self.last_prediction > 0 and prediction > 0 or self.last_prediction < 0 and prediction < 0 : 
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

        