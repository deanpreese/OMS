import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from darts import TimeSeries
from darts.dataprocessing.transformers import Scaler
from darts.models import NBEATSModel
from darts.metrics import mae
from sklearn.model_selection import train_test_split

# Data loading
def load_data(file_path):
    data = pd.read_csv(file_path)
    return data

# Preprocessing
def preprocess_data(data, feature_columns, target_column):
    scaler = Scaler()
    data[feature_columns] = scaler.fit_transform(data[feature_columns])
    data[target_column] = scaler.fit_transform(data[target_column].values.reshape(-1, 1))
    return data, scaler

# Create TimeSeries
def create_timeseries(data, target_column):
    series = TimeSeries.from_dataframe(data, time_col=None, value_cols=target_column)
    return series

# Plot results
def plot_results(actuals, predictions):
    plt.figure(figsize=(12, 6))
    plt.plot(actuals, label='Actual')
    plt.plot(predictions, label='Predicted')
    plt.xlabel('Time')
    plt.ylabel('Stock Price')
    plt.title('Stock Price Prediction')
    plt.legend()
    plt.show()

# Main function to run the entire script
def main():
    # Load and preprocess data
    data = load_data("data/buildSeqInd_Lucky13_5M_ALL.csv")
    data = data.drop(columns=['outputC'])
    feature_columns = list(data.columns[:-1])
    target_column = data.columns[-1]
    data, scaler = preprocess_data(data, feature_columns, target_column)
    
    # Split data into train and test sets
    train_data, test_data = train_test_split(data, test_size=0.2, shuffle=False)
    
    # Create TimeSeries
    train_series = create_timeseries(train_data, target_column)
    test_series = create_timeseries(test_data, target_column)
    
    # Build and train the N-BEATS model
    model = NBEATSModel(input_chunk_length=60, output_chunk_length=1, n_epochs=50, random_state=42)
    model.fit(train_series)
    
    # Make predictions
    predictions = model.predict(len(test_series))
    
    # Inverse transform the predictions and actual values
    actual_values = scaler.inverse_transform(test_series.values().reshape(-1, 1)).reshape(-1)
    predicted_values = scaler.inverse_transform(predictions.values().reshape(-1, 1)).reshape(-1)
    
    # Plot the results
    plot_results(actual_values, predicted_values)

    # Print the Mean Absolute Error (MAE)
    mae_score = mae(test_series, predictions)
    print(f'Mean Absolute Error: {mae_score:.4f}')

if __name__ == "__main__":
    main()
