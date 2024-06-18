import pandas as pd
import numpy as np
from scipy.cluster import hierarchy
import matplotlib.pyplot as plt
from sklearn.preprocessing import MinMaxScaler
from sklearn.feature_selection import SelectFromModel, RFECV
from sklearn.linear_model import LinearRegression
from sklearn.metrics import mean_squared_error
from sklearn.model_selection import train_test_split
from sklearn import datasets, ensemble
from sklearn.inspection import permutation_importance
from sklearn.preprocessing import MinMaxScaler, StandardScaler
#from xgboost import XGBRegressor
from models.wrapped_models import TunableCatBoostRegressor, TunableLGBMRegressor, TunableXGBRegressor


def highest_lowest_correlations_over_windows(df, window_size, target_col):
    highest_correlations = {}
    lowest_correlations = {}

    # Initialize dictionaries with empty lists for each feature
    features = df.columns.drop(target_col)
    for feature in features:
        highest_correlations[feature] = []
        lowest_correlations[feature] = []

    # Calculate rolling correlations
    for start in range(0, len(df) - window_size + 1):
        window_df = df.iloc[start:start + window_size]
        corr_matrix = window_df.corr()
        target_corr = corr_matrix[target_col].drop(target_col)

        for feature in features:
            corr_value = target_corr[feature]
            highest_correlations[feature].append(corr_value)
            lowest_correlations[feature].append(corr_value)
            
            print(f"Feature {feature}  Corr {corr_value}")

    # Find highest and lowest correlation for each feature
    highest_lowest = {
        'Feature': [],
        'Highest Correlation': [],
        'Lowest Correlation': []
    }
    for feature in features:
        highest_lowest['Feature'].append(feature)
        highest_lowest['Highest Correlation'].append(max(highest_correlations[feature]))
        highest_lowest['Lowest Correlation'].append(min(lowest_correlations[feature]))

    return pd.DataFrame(highest_lowest)


#file_loaded = pd.read_csv("data/Fractal_ALL_5M_orig.csv")

file_loaded = pd.read_csv("data/CleanReFried_5M_ALL.csv")
#file_loaded = file_loaded.drop(columns=['outputC'])

#file_loaded = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
#file_loaded = file_loaded.drop(columns=['outputC'])

#file_loaded = pd.read_csv("data/IND_LSTM_ALL.csv")
file_loaded = file_loaded.drop(columns=['outputC'])

feature_columns = list(file_loaded.columns[:-1])
num_columns = len(file_loaded.axes[1]) 
input_features =  num_columns -1
X = file_loaded.iloc[:, 0:input_features]  
y = file_loaded['output'].values

X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.3, random_state=42)

window_size = 25000
target_col = 'output'
highest_lowest_correlations = highest_lowest_correlations_over_windows(file_loaded, window_size, target_col)


params = TunableXGBRegressor().param_set()

base_model = TunableXGBRegressor(**params)
mod_sel = TunableXGBRegressor(**params)
post_sel = TunableXGBRegressor(**params)

select = SelectFromModel(mod_sel, threshold="median")
select.fit(X_train, y_train)
X_train_rf = select.transform(X_train)
X_test_rf = select.transform(X_test)

base_model.fit(X_train, y_train)
base_preds = base_model.predict(X_test)
base_score = base_model.score(X_test, y_test)
base_mse = mean_squared_error(y_test, base_preds)
base_rmse = base_mse**.5

post_sel.fit(X_train_rf, y_train)
post_sel_preds = post_sel.predict(X_test_rf)
post_sel_score = post_sel.score(X_test_rf, y_test)
post_mse = mean_squared_error(y_test, post_sel_preds)
post_rmse = post_mse**.5

pi_result = permutation_importance(
    post_sel, X_test_rf, y_test, n_repeats=10, random_state=42, n_jobs=2
)



print(" ******************************************")
print("")
print(f"Orig Shape {X_train.shape}")
print(f"Selected Shape {X_train_rf.shape}")
print("")
print("Base Model")
print(f"Score {base_score}  MSE {base_mse}  RMSE {base_rmse}") 
print("")
print("Post Sel Model")
print(f"Score {post_sel_score}  MSE {post_mse}  RMSE {post_rmse}") 
print("")
print(f"Selected features: {X.columns[select.get_support()]}")
print("")

print("Correlations")
print(highest_lowest_correlations)
print("")
print("Feature Importances")
feature_importance = post_sel.feature_importances_
feature_importancex = list(zip(list(X.columns[select.get_support()]), feature_importance))
sorted_feature_importance = sorted(feature_importancex, key=lambda x: x[1], reverse=True)

features_list_80 = []
features_list_60 = []
weight_total_60 = 0
weight_total_80 = 0

for feature, weight in sorted_feature_importance:
    print(f"{feature} {weight}")
    
    if weight_total_80 < 0.8:
        weight_total_80 += weight
        features_list_80.append(feature)
        
    if weight_total_60 < 0.6:
        weight_total_60 += weight
        features_list_60.append(feature)


print(" ")
print(f"Number of Features selected {len(X.columns[select.get_support()])}")
print(f"80% List {len(features_list_80)}")    
print(features_list_80)    
print(f"60% List {len(features_list_60)}")    
print(features_list_60)    
print(" ")

print("Permutation Importance")
sorted_idx = np.argsort(feature_importance)
pos = np.arange(sorted_idx.shape[0]) + 0.5
for ix in pi_result.importances_mean.argsort()[::-1]:
    print(f"{list(file_loaded.columns)[ix]} {pi_result.importances_mean[ix]}  {pi_result.importances_std[ix]}")

print(" ")

