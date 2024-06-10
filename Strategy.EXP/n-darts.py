import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from darts import TimeSeries
from darts.dataprocessing.transformers import Scaler
from darts.models import NBEATSModel
from darts.metrics import mae, mape, rmse
import joblib

# Parameters
FILE_PATH = "data/buildSeqInd_Lucky13_5M_ALL.csv"
TARGET_COLUMN = 'output'  # Replace with your actual target column name
INPUT_CHUNK_LENGTH = 60
OUTPUT_CHUNK_LENGTH = 1
N_EPOCHS = 20
NUM_STACKS = 2
NUM_BLOCKS = 1
NUM_LAYERS = 4
LAYER_WIDTHS = 512
TEST_SPLIT_RATIO = 0.8
MODEL_SAVE_PATH = "nbeats_model.pkl"

# Data loading
def load_data(file_path):
    data = pd.read_csv(file_path)
    return data

# Preprocessing
def preprocess_data(data, feature_columns, target_column):
    series = TimeSeries.from_dataframe(data, value_cols=target_column).astype(np.float32)
    scaler = Scaler()
    scaled_series = scaler.fit_transform(series)
    return scaled_series, scaler

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
    data = load_data(FILE_PATH)
    data = data.drop(columns=['outputC'])
    feature_columns = list(data.columns[:-1])
    target_column = TARGET_COLUMN
    
    # Create TimeSeries and preprocess data
    series = TimeSeries.from_dataframe(data, value_cols=target_column).astype(np.float32)
    train_data, test_data = series.split_after(TEST_SPLIT_RATIO)
    train_series, scaler = preprocess_data(train_data.pd_dataframe(), feature_columns, target_column)
    test_series = scaler.transform(test_data.astype(np.float32))
    
    # Build and train the N-BEATS model
    model = NBEATSModel(
        input_chunk_length=INPUT_CHUNK_LENGTH, 
        output_chunk_length=OUTPUT_CHUNK_LENGTH, 
        n_epochs=N_EPOCHS, 
        random_state=42, 
        num_stacks=NUM_STACKS, 
        num_blocks=NUM_BLOCKS, 
        num_layers=NUM_LAYERS, 
        layer_widths=LAYER_WIDTHS
    )
    model.fit(train_series)
    
    # Save the trained model
    joblib.dump(model, MODEL_SAVE_PATH)
    print(f"Model saved to {MODEL_SAVE_PATH}")
    
    # Make predictions
    predictions = model.predict(len(test_series))
    
    # Inverse transform the predictions and actual values
    actual_values = scaler.inverse_transform(test_series).values()
    predicted_values = scaler.inverse_transform(predictions).values()
    
    # Plot the results
    plot_results(actual_values, predicted_values)

    # Print descriptive statistics for model performance
    mae_score = mae(test_series, predictions)
    mape_score = mape(test_series, predictions)
    rmse_score = rmse(test_series, predictions)
    print(f'Mean Absolute Error (MAE): {mae_score:.4f}')
    print(f'Mean Absolute Percentage Error (MAPE): {mape_score:.4f}%')
    print(f'Root Mean Squared Error (RMSE): {rmse_score:.4f}')

if __name__ == "__main__":
    main()
