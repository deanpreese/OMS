import json
import requests
import pulsar

class PulsarConsumerCli:
    def __init__(self, **kwargs):
        
        PULSAR_NEW_ORDER_TOPIC =  "persistent://public/models/new-model-orders"
        self.topic = PULSAR_NEW_ORDER_TOPIC
        #self.client = pulsar.Client('pulsar://10.0.0.82:6650')
        self.client = pulsar.Client('pulsar://10.0.0.50:6650')
       

    def get_consumer(self, subs):
        return  self.client.subscribe(self.topic, subs)


class PulsarProducerCli:
    def __init__(self, **kwargs):
        
        PULSAR_NEW_ORDER_TOPIC =  "persistent://public/models/new-model-orders"
        self.topic = PULSAR_NEW_ORDER_TOPIC
        #self.client = pulsar.Client('pulsar://10.0.0.82:6650')
        self.client = pulsar.Client('pulsar://10.0.0.50:6650')
        self.producer = self.client.create_producer(self.topic)
                  
    def send_order_pulsar(self, new_order):
        
        #self.producer = self.client.create_producer(self.topic)
        data=json.dumps(new_order,default=str)
        self.producer.send(data.encode('utf-8'))


class CommonCli:
               

    def initialize_trader(display_name, group_num ):
        
        t_id = 0
        
        url = "http://10.0.0.147:8786/api/ml/verify-model-trader"  # Replace with the actual URL of the web service

        headers = {
            "Content-Type": "application/json"
        }

        add_trader_model = {  
            "group": group_num,
            "userId": 0,
            "displayName": display_name,
            "userPwd": "abc",
            "firstName": "Model",
            "lastName": "Trader",
            "email": "bac.abc"
        }
                        
        response = requests.post(url, data=json.dumps(add_trader_model), headers=headers)

        if response.status_code == 200:
            result = response.json()
            t_id = result
            
        else:
            # Error handling
                print(f"Trader initialize request failed with status code {response.status_code}")

        return t_id
                        
  
                    

    def send_order(self, new_order):
        
        url = "http://10.0.0.147:8786/api/ml/process-order"  # Replace with the actual URL of the web service

        headers = {
            "Content-Type": "application/json"
        }

        response = requests.post(url, data=json.dumps(new_order,default=str), headers=headers)
        if response.status_code == 200:
            pass
        else:
            print(f"New Order send failed with status code {response.text}")                
                