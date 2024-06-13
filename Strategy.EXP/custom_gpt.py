import numpy as np
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from sklearn.metrics import mean_squared_error
import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout, Input
from tensorflow.keras.optimizers import Adam
from lightgbm import LGBMRegressor
from darts.models import NHiTSModel
from darts import TimeSeries


tf.config.set_visible_devices([], 'GPU')

# Load and preprocess data
def load_data():
    data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
    features = data.drop(['output', 'outputC'], axis=1)
    target = data['output']
    
    scaler = MinMaxScaler()
    features_scaled = scaler.fit_transform(features)
    
    return features_scaled, target

# Prepare LSTM model
def create_lstm_model(input_dim):
    model = Sequential([
        Input(shape=(1, input_dim)),
        LSTM(50, return_sequences=True),
        Dropout(0.2),
        LSTM(50),
        Dropout(0.2),
        Dense(1)
    ])
    model.compile(optimizer=Adam(learning_rate=0.01), loss='mse')
    return model

# Main function to orchestrate the ensemble modeling
def main():
    features, target = load_data()
    X_train, X_test, y_train, y_test = train_test_split(features, target, test_size=0.2, random_state=42)
    
    # Reshape for LSTM
    X_train_lstm = X_train.reshape((X_train.shape[0], 1, X_train.shape[1]))
    X_test_lstm = X_test.reshape((X_test.shape[0], 1, X_test.shape[1]))

    # Create and train LightGBM model
    lgbm_model = LGBMRegressor(n_estimators=100, verbose = 1)
    lgbm_model.fit(X_train, y_train)

    # Create and train LSTM model
    lstm_model = create_lstm_model(X_train.shape[1])
    lstm_model.fit(X_train_lstm, y_train, epochs=10, batch_size=32, verbose=1, validation_split=0.1)


    # Create and train NHiTS model
    y_train_ts = TimeSeries.from_series(pd.Series(y_train))
    n_hits_model = NHiTSModel(input_chunk_length=15, output_chunk_length=5, n_epochs=100)
    n_hits_model.fit(series=y_train_ts)

    # Ensemble prediction function
    def ensemble_predictions(X):
        # Predict with LSTM
        X_lstm = X.reshape((X.shape[0], 1, X.shape[1]))
        lstm_pred = lstm_model.predict(X_lstm).flatten()

        # Predict with LightGBM
        lgbm_pred = lgbm_model.predict(X)

        # Predict with NHiTS
        X_ts = TimeSeries.from_dataframe(pd.DataFrame(X))
        n_hits_pred = n_hits_model.predict(n=len(X)).values().flatten()

        # Average predictions
        return np.mean([lstm_pred, lgbm_pred, n_hits_pred], axis=0)

    # Evaluate ensemble
    predictions = ensemble_predictions(X_test)
    mse = mean_squared_error(y_test, predictions)
    print(f'Ensemble MSE: {mse}')

if __name__ == "__main__":
    main()
