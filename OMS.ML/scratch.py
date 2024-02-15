



exit()
grid = {
       'learning_rate' : [ 0.009, 0.01],
        #'depth' : [3,5,7,9,11],
        'depth' : [3,7,9],
        #'l2_leaf_reg' : [1.0, 1.5, 2.0, 2.5, 3.0, 3.5, 4.0, 4.5, 5.0 ],
        'l2_leaf_reg' : [ 3.0, 4.0, 5.0 ],
        'min_child_samples' : [1, 4, 8, 16, 32],
        'min_child_samples' : [16,32],
        #'grow_policy' : ['Depthwise'],
        'iterations' : [1000],
        #'eval_metric' : ['RMSE'],
        'random_state' : [0],
        #'boosting_type' : ['Ordered', 'Plain'],
        'thread_count' : [-1],
    }


param_combinations = list(product(*grid.values()))

par_list = []

for params in param_combinations:
    param_set = dict(zip(grid.keys(), params))
    
    par_list.append(param_set)
   
for p in par_list:
    print(p)


print(len(par_list))




"""
    shodata = False
    if ( shodata ):
        
        print(" ")
        print("Columns Used in Model")
        print(f" {len(X.columns)}  ")
        print("Column Names ")
        print(X.columns)
        print(" ")

        full = pd.concat([X_test.reset_index(drop=True), 
            pd.DataFrame(y_test).reset_index(drop=True), 
            pd.DataFrame({'preds':predicted_values})], axis=1)

        #print(f"{full}")

        # Calculate correlations
        correlations = full.corr()

        # Display the correlation matrix
        print("Correlation Matrix:")
        print(correlations)

        #correlations.to_csv("output/correlations.csv")

        # Plot the correlation matrix as a heatmap
        plt.figure(figsize=(16, 9))
        sns.heatmap(correlations, annot=True, cmap='coolwarm', fmt=".2f", linewidths=.5)
        plt.title('Correlation Matrix')
        #plt.show()


        # Create a DataFrame with feature importance
        importance_types = ['weight', 'gain', 'cover', 'total_gain', 'total_cover']
        feature_importance_df = pd.DataFrame()

        # Iterate through each importance type and store the results in the DataFrame
        for importance_type in importance_types:
            importance_values = r.model.get_booster().get_score(importance_type=importance_type)
            importance_df = pd.DataFrame(list(importance_values.items()), columns=['Feature', importance_type])
            feature_importance_df = pd.concat([feature_importance_df, importance_df], axis=1)

        # Display the feature importance DataFrame
        fidf = feature_importance_df.sort_values(by=['weight'],  ascending=False)
        #fidf.to_csv("output/feature_importance_df.csv")
        
        print("Feature Importance:")
        print(fidf)         
"""
