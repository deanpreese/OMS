import json
import requests
from datetime import datetime as dt
from datetime import timedelta

class OrderManager():
    
    def __init__(self):
        self.px = 0
        self.commission = 0
        self.tick_dte = dt(1, 1, 1)

    def process_tick_rt(self, tick, ticks):
        self.px = tick    
        self.tick_dte = ticks
        
    def process_model(self, model, y_pred, prediction):

        orders = model.check_for_orders(prediction, self.px)        
        
        if len(orders) > 0:        
             for o in orders:
                self.send_order(o)
        
 

    def close_all(self, model):
        
        o_t_c = model.close_orders(self.px)
        
        if len(o_t_c) > 0:
             for o in o_t_c:
                self.send_order(o)


    def send_order(self, new_order):

        dte_iso = self.tick_dte
        new_order['orderTime']  = dte_iso
                
        if self.px > 0:
        
            print(f" Order   {new_order['userID']}   {new_order['userName']}   {new_order['orderAction']}  {new_order['orderPX']} {new_order['orderTime']} " ) 

            url = "http://localhost:8786/api/ml/process-order"  # Replace with the actual URL of the web service

            headers = {
                "Content-Type": "application/json"
            }

            response = requests.post(url, data=json.dumps(new_order,default=str), headers=headers)
            if response.status_code == 200:
                pass
            else:
                print(f"New Order send failed with status code {response.text}")                
