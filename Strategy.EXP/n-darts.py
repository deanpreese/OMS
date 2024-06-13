import pandas as pd
import numpy as np
import datetime as dte_time
import matplotlib.pyplot as plt
from darts import TimeSeries
from darts.dataprocessing.transformers import Scaler
from darts.models import NHiTSModel
from pytorch_lightning.callbacks import EarlyStopping, LearningRateMonitor
from pytorch_lightning.loggers import TensorBoardLogger
from darts.utils.likelihood_models import QuantileRegression
from darts.metrics import mae, mape, rmse, coefficient_of_variation, dtw_metric
import joblib

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

def generate_statistics(test_series, predictions):
    # Print descriptive statistics for model performance
    mae_score = mae(test_series, predictions)
    mape_score = mape(test_series, predictions)
    rmse_score = rmse(test_series, predictions)
    cov = coefficient_of_variation(test_series, predictions)
    dtw = dtw_metric(test_series, predictions)
    
    print(f'Dynamic Time Warping (DTW): {dtw:.4f}')
    print(f'Coefficient of Variation (CoV): {cov:.4f}')
    print(f'Mean Absolute Error (MAE): {mae_score:.4f}')
    print(f'Mean Absolute Percentage Error (MAPE): {mape_score:.4f}%')
    print(f'Root Mean Squared Error (RMSE): {rmse_score:.4f}')

def train_and_save_model(train_series, val_series, input_chunk_length, output_chunk_length, 
                         n_epochs, num_stacks, num_blocks, num_layers, layer_widths, model_save_path, pl_trainer_kwargs):
    # Build and train the NHiTS model
    model = NHiTSModel(
        input_chunk_length=input_chunk_length, 
        output_chunk_length=output_chunk_length, 
        n_epochs=n_epochs,
        batch_size=64, 
        random_state=42, 
        num_stacks=num_stacks, 
        num_blocks=num_blocks, 
        num_layers=num_layers, 
        layer_widths=layer_widths,
        pl_trainer_kwargs=pl_trainer_kwargs,
        likelihood=QuantileRegression(),
        log_tensorboard=True
    )
    
    model.fit(train_series, val_series=val_series)
    model.save(model_save_path)

    return model

# Main function to run the entire script
def main():
    file_path = "data/buildSeqInd_Lucky13_5M_ALL.csv"
    drop_cols = [
        #'STOK1',
        #'RSI',
        #'ATR2',
        'ATR21',
        #'ATR3',
        'ATR31', 
        'ATR32',
        'ATR34',   
        #'ROC',     
        'SDKC9',   
        'SDKC91',  
        'SDBB91',  
        #'SDLR310'
    ]

    target_column = 'output'  # Replace with your actual target column name
    input_chunk_length = 13
    output_chunk_length = 2
    n_epochs = 1000
    num_stacks = 3
    num_blocks = 2
    num_layers = 4
    layer_widths = 512
    test_split = 0.85

    time_stamp = dte_time.datetime.utcnow().strftime('%Y%m%d%H%M%S%f')
    model_base_name = f"dart_NHiTSModel_{input_chunk_length}-{output_chunk_length}_{time_stamp}"
    model_save_path = f"dart_logs/{model_base_name}.pk"
    
    # Load and preprocess data
    data = load_data(file_path)
    #data = data.drop(columns=['outputC'])
    data = data.drop(columns=drop_cols)
    feature_columns = list(data.columns[:-1])
    
    # Create TimeSeries and preprocess data
    series = TimeSeries.from_dataframe(data, value_cols=target_column).astype(np.float32)
    train_data, test_data = series.split_after(test_split)
    train_series, scaler = preprocess_data(train_data.pd_dataframe(), feature_columns, target_column)
    test_series = scaler.transform(test_data.astype(np.float32))
    
    # TensorBoard logger
    lr_monitor = LearningRateMonitor(logging_interval='step')

    # Early stopping callback
    early_stopper = EarlyStopping(
        monitor="val_loss",
        patience=5,
        min_delta=0.01,
        verbose=True,
        mode='min'
    )

    pl_trainer_kwargs = {
        "callbacks": [early_stopper]
    }

    print("Training model...")
    model = train_and_save_model(train_series, test_series, input_chunk_length, output_chunk_length, 
                                 n_epochs, num_stacks, num_blocks, num_layers, layer_widths, model_save_path, pl_trainer_kwargs)
    
    # Make predictions
    print("Making predictions...")
    predictions = model.predict(len(test_series))
    print("Generating statistics...")
    generate_statistics(test_series, predictions)
    
    # Inverse transform the predictions and actual values
    actual_values = scaler.inverse_transform(test_series).values()
    predicted_values = scaler.inverse_transform(predictions).values()
    plot_results(actual_values, predicted_values)

if __name__ == "__main__":
    main()
