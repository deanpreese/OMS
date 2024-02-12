

class SimOrderManager():
    
    def __init__(self):
        self.px = 2200
        self.commission = 0
    
    def process_tick(self, tick):
        self.px += tick    

    def process_order(self, model, y_pred, preditcion):

        if (preditcion > 0 and y_pred > 0  or
            preditcion < 0 and y_pred < 0  or 
            preditcion == 0 and y_pred == 0 ):
        
            print(f"OK      {preditcion}     { y_pred }    {self.px}" )

            model.winners += abs(y_pred) - self.commission
            model.win_cnt += 1

        else:
            print(f"WRONG   {preditcion}     { y_pred }    {self.px}" )            
            model.losses += abs(  y_pred  ) + self.commission
            model.loss_cnt += 1

 
       