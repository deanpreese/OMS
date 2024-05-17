import pandas as pd
import numpy as np
from sklearn.preprocessing import MinMaxScaler
import tensorflow as tf


# Read the data into a DataFrame
df = pd.read_csv('data/lucky13_oos.csv')

# Drop the outputC column
df = df.drop(columns=['outputC'])

# Normalize the data
scaler = MinMaxScaler()
scaled_data = scaler.fit_transform(df)
df = pd.DataFrame(scaled_data, columns=df.columns)

# Convert data to sequences
def create_sequences(data, seq_length):
    xs = []
    for i in range(len(data) - seq_length):
        x = data.iloc[i:(i+seq_length), :-1]
        xs.append(x.values)
    return np.array(xs)

seq_length = 5
X = create_sequences(df, seq_length)

# Load the saved LSTM model
model = tf.keras.models.load_model('lstm-50-50-5.keras')


# Generate incremental one-by-one predictions
predictions = []
input_seq = X[0]  # Start with the first sequence


for _ in range(len(X)):
    input_seq_reshaped = input_seq.reshape((1, seq_length, X.shape[2]))
    pred = model.predict(input_seq_reshaped)
    
    print(input_seq_reshaped)
    print(pred)
    
    predictions.append(pred[0][0])
    
    # Append the prediction to the sequence and remove the oldest value
    next_seq = np.append(input_seq[1:], [[pred[0][0]]], axis=0)
    
    input_seq = next_seq

# Print the predictions
print(f"Predict {predictions}  " ) 
