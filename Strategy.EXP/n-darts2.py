import pandas as pd
import numpy as np
import datetime as dte_time
import matplotlib.pyplot as plt
from darts import TimeSeries
from darts.dataprocessing.transformers import Scaler
from darts.models import NHiTSModel
from pytorch_lightning.callbacks import EarlyStopping, LearningRateMonitor
from darts.utils.likelihood_models import QuantileRegression
from darts.metrics import mae, mape, rmse, coefficient_of_variation, dtw_metric
import joblib

# Data loading
def load_data(file_path):
    data = pd.read_csv(file_path)
    return data

# Preprocessing
def preprocess_data(data, target_column):
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
                         n_epochs, num_stacks, num_blocks, num_layers, 
                         layer_widths, model_save_path, scaler_save_path, scaler, pl_trainer_kwargs):
    
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
    joblib.dump(scaler, scaler_save_path)

    return model

def load_model_and_scaler(model_path, scaler_path):
    model = NHiTSModel.load(model_path)
    scaler = joblib.load(scaler_path)
    return model, scaler

def loop_predictions(model, series, input_chunk_length, output_chunk_length):
    predictions = []
    for i in range(0, len(series) - input_chunk_length, output_chunk_length):
        input_series = series[i:i + input_chunk_length]
        prediction = model.predict(output_chunk_length, input_series)
        predictions.extend(prediction.values().flatten())
    return np.array(predictions)

# Main function to run the entire script
def main():
    file_path = "data/buildSeqInd_Lucky13_5M_ALL.csv"
    drop_cols = [
        'ATR21',
        'ATR31', 
        'ATR32',
        'ATR34',   
        'SDKC9',   
        'SDKC91',  
        'SDBB91',  
    ]

    target_column = 'output'
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
    scaler_save_path = f"dart_logs/{model_base_name}_scaler.pkl"
    
    # Load and preprocess data
    data = load_data(file_path)
    data = data.drop(columns=drop_cols)
    feature_columns = list(data.columns[:-1])
    
    # Create TimeSeries and preprocess data
    series = TimeSeries.from_dataframe(data, value_cols=target_column).astype(np.float32)
    train_data, test_data = series.split_after(test_split)
    train_series, scaler = preprocess_data(train_data.pd_dataframe(), target_column)
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
                                 n_epochs, num_stacks, num_blocks, num_layers, 
                                 layer_widths, model_save_path, scaler_save_path,scaler, pl_trainer_kwargs)
    
    print("Making predictions...")
    predictions = loop_predictions(model, test_series, input_chunk_length, output_chunk_length)
    print("Generating statistics...")
    generate_statistics(test_series, predictions)
    
    # Inverse transform the predictions and actual values
    actual_values = scaler.inverse_transform(test_series).values()
    predicted_values = scaler.inverse_transform(predictions).values()
    plot_results(actual_values, predicted_values)

    # Load out-of-sample data and make predictions
    out_of_sample_file_path = "data/out_of_sample_data.csv"
    out_of_sample_data = load_data(out_of_sample_file_path)
    out_of_sample_data = out_of_sample_data.drop(columns=drop_cols)
    out_of_sample_series = TimeSeries.from_dataframe(out_of_sample_data, value_cols=target_column).astype(np.float32)
    
    model, scaler = load_model_and_scaler(model_save_path, scaler_save_path)
    scaled_out_of_sample_series = scaler.transform(out_of_sample_series)
    
    out_of_sample_predictions = loop_predictions(model, scaled_out_of_sample_series, input_chunk_length, output_chunk_length)
    
    # Inverse transform the out-of-sample predictions and actual values
    actual_out_of_sample_values = scaler.inverse_transform(scaled_out_of_sample_series).values()
    predicted_out_of_sample_values = scaler.inverse_transform(out_of_sample_predictions).values()
    plot_results(actual_out_of_sample_values, predicted_out_of_sample_values)

if __name__ == "__main__":
    main()