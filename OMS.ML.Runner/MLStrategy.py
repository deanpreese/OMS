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



class MLStrategy:
    def __init__(self, model, col_filter, run_id):
        self.model = model
        self.column_filter = col_filter
        self.run_id = run_id
        self.run_name = ""
        self.trader_id = 0
        self.trader_group = 55
        
        self.winners = 0
        self.losses = 0
        self.win_cnt = 0
        self.loss_cnt = 0
                
        self.last_prediction = 0
        self.position = position_status.FLAT
        self.bars_since = 0
        
        self.bars_exit = 5
        self.entry_px = 0
        self.profit = 5
        self.stop = 3
        
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
        
        if prediction > 0:
            new_order_action = order_action.Buy
        if prediction < 0:
            new_order_action = order_action.Sell

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
            
                if self.check_long_pl(px) or self.check_bars_since(px) :
                    print(f"Close Long PnL or bars {self.bars_exit}  {px}") 
                    orders.append(self.close_this_order(px))
            
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
                
                if self.check_short_pl(px) or self.check_bars_since(px) :
                    print(f"Close Long PnL or bars {self.bars_exit}  {px}") 
                    orders.append(self.close_this_order(px))
            
            # short but buy order
            if new_order_action == order_action.Buy:
                orders.append(self.build_order(new_order_action,px))  
                self.last_prediction = prediction
                self.position = position_status.FLAT
                self.bars_since = 0
   
   
        return orders

    def check_bars_since(self, px):
        
        if self.bars_since > self.bars_exit:
            print(f"Close bars greater than {self.bars_exit}  {px}") 
            return True
        return False
        
    def check_short_pl(self, px):
        
        if self.entry_px > px and abs(self.entry_px - px) > self.profit: 
            return True
        
        if self.entry_px < px and abs(self.entry_px - px) > self.stop:
            return True
        
        return False    
    
    def check_long_pl(self, px):
        
        if self.entry_px < px and abs(self.entry_px - px) > self.profit: 
            return True
        
        if self.entry_px > px and abs(self.entry_px - px) > self.stop:
            return True
    
        return False
    
    def close_this_order(self, px):

        if self.position == position_status.LONG:
            self.position = position_status.FLAT
            self.bars_since = 0
            return self.build_order(order_action.Sell,px)
            
        if self.position == position_status.SHORT:
            self.position = position_status.FLAT
            self.bars_since = 0  
            return self.build_order(order_action.Buy,px)
           
    
    # --------------------------------------
    #  Called by OM to close all open orders    
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
        self.entry_px = px
        
        new_order = {
            "newOrderID": 0,
            "platformOrderID": 0,
            "userName": self.run_name,
            "userID": int(self.trader_id),
            "userGroup": int(self.trader_group),
            "authToken": 0,
            "instrument": "ES",
            "orderPX": px,
            "orderType": "Market",
            "orderAction": int(new_order_action.value),
            "quantity": 1,
            "orderTime": dte_iso,
        }    
    
        return new_order

        