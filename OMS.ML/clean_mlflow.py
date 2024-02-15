
import mlflow

el = mlflow.search_experiments()

for e in el:
    eid = e.experiment_id
    print(eid)
    
    if eid != 0:
        mlflow.delete_experiment(eid)
