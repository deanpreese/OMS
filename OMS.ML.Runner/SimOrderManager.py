import json
import requests

from datetime import datetime, timedelta
from common.CommonCli import CommonCli



class SimOrderManager():
    
    def __init__(self):
        self.px = 5000
        self.commission = 0
        self.tick_dte = datetime(2023, 1, 1, 12, 0)
        
        self.com_cli = CommonCli()
        
    
    def process_tick(self, tick):
        self.px += tick    
        self.tick_dte = self.tick_dte + timedelta(minutes=5)

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

        dte_iso = self.tick_dte.isoformat()
        new_order['orderTime']  = dte_iso

        print(f"Order   {new_order['userID']}   {new_order['userName']}   {new_order['orderAction']}  {new_order['orderPX']} {new_order['orderTime']} " ) 
        
        CommonCli.send_order(new_order)
        