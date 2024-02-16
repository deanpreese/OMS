from enum import Enum
import json
import mlflow
import pandas as pd
import requests
import datetime as dt
import random as rand
from models import wrapped_models


class position_status(Enum):
    FLAT = 0
    LONG = 1
    SHORT = -1



class order_action(Enum):
    Buy = 2
    Sell = -2



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
        self.position = position_status.FLAT
        self.bars_since = 0
        
        
        
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
        
        if prediction > 0 : new_order_action = order_action.Buy
        if prediction < 0 : new_order_action = order_action.Sell
        
        orders = []
        
        if self.position == position_status.FLAT:
            orders.append(self.build_order( new_order_action,px))
            
            if new_order_action == order_action.Buy:
                self.position = position_status.LONG 

            if new_order_action == order_action.Sell:
                self.position = position_status.SHORT

            self.last_prediction = prediction
            self.bars_since = 1
            
            
        if self.position == position_status.LONG:
            
            # already long dont add
            if  new_order_action == order_action.Buy:
                self.last_prediction = prediction
                self.bars_since += 1
            
            # long but sell order
            if new_order_action == order_action.Sell:
                orders.append(self.build_order(new_order_action,px))  
                self.last_prediction = prediction
                self.position = position_status.FLAT
                self.bars_since = 0
                
        
        if self.position == position_status.SHORT:

            # already short dont add
            if  new_order_action == order_action.Sell:
                self.last_prediction = prediction
                self.bars_since += 1
            
            # long but sell order
            if new_order_action == order_action.Buy:
                orders.append(self.build_order(new_order_action,px))  
                self.last_prediction = prediction
                self.position = position_status.FLAT
                self.bars_since = 0
    
    
        if self.bars_since > 5:
            orders.append(self.close_this_order(px)) 
            self.bars_since = 0 
            self.position = position_status.FLAT 
    
        return orders
    
    
    def close_this_order(self, px):
        
        otc = None
        
        if self.position == position_status.LONG:
            otc = self.build_order(order_action.Sell,px)
            self.position = position_status.FLAT
            
        if self.position == position_status.SHORT:
            otc = self.build_order(order_action.Buy,px)
            self.position = position_status.FLAT  
    
        return otc
    
    def close_orders(self, px):
       
        orders_to_close = []
        
        if self.position == position_status.LONG:
            orders_to_close.append( self.build_order(order_action.Sell,px))
            self.position = position_status.FLAT
            
        if self.position == position_status.SHORT:
            orders_to_close.append( self.build_order(order_action.Buy,px))
            self.position = position_status.FLAT  

        return orders_to_close
    
    
    def build_order(self, new_order_action, px):
        
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
            "orderAction": new_order_action.value,
            "quantity": 1,
            "orderTime": dte_iso,
        }    
    
        return new_order

        