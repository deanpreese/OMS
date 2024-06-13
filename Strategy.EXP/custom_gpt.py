from pandas import date_range
import numpy as np
import pandas as pd
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler
from sklearn.metrics import mean_squared_error
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout, Input
from tensorflow.keras.optimizers import Adam
from lightgbm import LGBMRegressor
from darts.models import NHiTSModel
from darts import TimeSeries

def load_data():
    data = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
    features = data.drop(['output', 'outputC'], axis=1)
    target = data['output']
    scaler = MinMaxScaler()
    features_scaled = scaler.fit_transform(features)
    return features_scaled, target

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

def main():
    features, target = load_data()
    X_train, X_test, y_train, y_test = train_test_split(features, target, test_size=0.2, random_state=42)
    
    # Check data size and adjust frequency
    if len(y_train) > 10000:
        print("Warning: Large dataset size. Using monthly frequency.")
        freq = 'M'  # Monthly frequency
    else:
        freq = 'D'  # Daily frequency

    train_dates = date_range(start='2020-01-01', periods=len(y_train), freq=freq)
    test_dates = date_range(start='2020-01-01', periods=len(X_test), freq=freq)

    X_train_lstm = X_train.reshape((X_train.shape[0], 1, X_train.shape[1]))
    X_test_lstm = X_test.reshape((X_test.shape[0], 1, X_test.shape[1]))

    lstm_model = create_lstm_model(X_train.shape[1])
    lstm_model.fit(X_train_lstm, y_train, epochs=50, batch_size=32, verbose=1, validation_split=0.1)

    lgbm_model = LGBMRegressor(n_estimators=100)
    lgbm_model.fit(X_train, y_train)

    y_train_ts = TimeSeries.from_dataframe(pd.DataFrame(data={'value': y_train}, index=train_dates))
    n_hits_model = NHiTSModel(input_chunk_length=15, output_chunk_length=5, n_epochs=100)
    n_hits_model.fit(series=y_train_ts)

    def ensemble_predictions(X, X_dates):
        X_lstm = X.reshape((X.shape[0], 1, X.shape[1]))
        lstm_pred = lstm_model.predict(X_lstm).flatten()
        lgbm_pred = lgbm_model.predict(X)
        X_ts = TimeSeries.from_dataframe(pd.DataFrame(data={'value': np.zeros_like(X[:, 0])}, index=X_dates))
        n_hits_pred = n_hits_model.predict(n=len(X), series=X_ts).values().flatten()
        return np.mean([lstm_pred, lgbm_pred, n_hits_pred], axis=0)

    predictions = ensemble_predictions(X_test, test_dates)
    mse = mean_squared_error(y_test, predictions)
    print(f'Ensemble MSE: {mse}')

if __name__ == "__main__":
    main()
