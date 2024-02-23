import json
import requests

from datetime import datetime, timedelta

class SimOrderManager():
    
    def __init__(self):
        self.px = 5000
        self.commission = 0
        self.tick_dte = datetime(2023, 1, 1, 12, 0)
    
    def convert_csharp_ticks_to_datetime(ticks):
        # C# ticks start from this date
        base_date = datetime(1, 1, 1)
        # There are 10,000,000 ticks in a second because each tick represents 100 nanoseconds
        ticks_per_second = 10000000
        # Convert ticks to seconds
        seconds = ticks / ticks_per_second
        # Add the seconds to the base date
        result_date = base_date + timedelta(seconds=seconds)
        return result_date
    
    
    def process_tick(self, tick):
        self.px += tick    
        self.tick_dte = self.tick_dte + timedelta(minutes=5)

    def process_tick_rt(self, tick):
        self.px += tick    
        self.tick_dte = self.tick_dte + timedelta(minutes=5)

    def process_model(self, model, y_pred, prediction):
        
        """
        if (prediction > 0 and y_pred > 0  or
            prediction < 0 and y_pred < 0  or 
            prediction == 0 and y_pred == 0 ):
        
            #print(f"OK      {prediction}     { y_pred }    {self.px}" )

            model.winners += abs(y_pred) - self.commission
            model.win_cnt += 1

        else:
            #print(f"WRONG   {prediction}     { y_pred }    {self.px}" )            
            model.losses += abs(  y_pred  ) + self.commission
            model.loss_cnt += 1
       
        """
        
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

        print(f" Order   {new_order['userID']}   {new_order['userName']}   {new_order['orderAction']}  {new_order['orderPX']} {new_order['orderTime']} " ) 
                
        url = "http://localhost:8786/api/ml/process-order"  # Replace with the actual URL of the web service

        headers = {
            "Content-Type": "application/json"
        }


        #response = requests.post(url, data=json.dumps(new_order), headers=headers)
        #if response.status_code == 200:
        #    pass
        #else:
        #    print(f"New Order send failed with status code {response.text}")                
                           
                           
                              