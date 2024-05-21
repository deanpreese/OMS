
import mlflow
mlflow.set_tracking_uri(uri="http://10.0.0.50:8888")

el = mlflow.search_experiments()

skip = True

for e in el:
    eid = e.experiment_id
    print(eid)
    
    if skip is False:
        mlflow.delete_experiment(eid)
        
    skip = False        
