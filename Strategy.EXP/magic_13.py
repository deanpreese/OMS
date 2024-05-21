import itertools
import pandas as pd
import xgboost as xgb
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score


# Function to train and evaluate an XGBoost model
def train_evaluate_xgboost(X_train, X_test, y_train, y_test):
    model = xgb.XGBClassifier()
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)
    return accuracy_score(y_test, y_pred)


train_file = pd.read_csv('data/sm13_3070.csv')
#train_file = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
data_loaded = train_file.drop(columns=['outputC'])
features = data_loaded.columns[:-1]
target = 'output'

# Generate all combinations of 13 features
feature_combinations = itertools.combinations(features, 2)

# Dictionary to store results
results = {}

# Fixed random state for reproducibility
random_state = 42

comb = 0

for i in range(2,len(features)):

    feature_combinations = itertools.combinations(features, i)

    for combination in feature_combinations:
        X = data_loaded[list(combination)]
        y = data_loaded[target]
        
        comb += 1
        
        print(f"Combination: {combination}")
        print(X)
        
        
        # Split the data
        #X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.3, random_state=random_state)
        
        # Train and evaluate the model
        #accuracy = train_evaluate_xgboost(X_train, X_test, y_train, y_test)
        
        # Store the result
        #results[combination] = accuracy


print(f"Total combinations: {comb}")    

"""
# Print the best combination based on accuracy
best_combination = max(results, key=results.get)
print(f"Best combination: {best_combination}")
print(f"Accuracy: {results[best_combination]}")

# If you want to save the results to a file
results_df = pd.DataFrame(list(results.items()), columns=['Combination', 'Accuracy'])
results_df.to_csv('xgboost_combinations_results.csv', index=False)
"""