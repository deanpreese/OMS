import numpy as np
import pandas as pd
import tensorflow as tf
import random
from tensorflow.keras.layers import Input, LSTM, Concatenate, Reshape, Flatten, Dense, LeakyReLU, Dropout, MultiHeadAttention
from tensorflow.keras.layers import   BatchNormalization, Layer,  Attention, Bidirectional, TimeDistributed, Conv1D, Conv2D
from tensorflow.keras.models import Model, Sequential
from tensorflow.keras.optimizers import Adam
from tensorflow.keras.initializers import RandomNormal
from tensorflow.keras.regularizers import l2
from keras.callbacks import EarlyStopping, ReduceLROnPlateau

from xgboost import XGBRegressor
from lightgbm import LGBMRegressor
import matplotlib.pyplot as plt
from sklearn.metrics import mean_squared_error, r2_score
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import MinMaxScaler, StandardScaler

tf.config.set_visible_devices([], 'GPU')


# Function to create sequences
def create_sequences(data_in, seq_length_in):
    xs = [data_in.iloc[i:i + seq_length_in, :-1].values for i in range(len(data_in) - seq_length_in)]
    ys = data_in.iloc[seq_length_in:, -1].values
    return np.array(xs), np.array(ys)


# Function to normalize sequences
def normalize_sequences(sequences_in):
    scalers_out = {}
    for i in range(sequences_in.shape[0]):
        #scalers_out[i] = MinMaxScaler((0,1))
        scalers_out[i] = StandardScaler() 
        sequences_in[i] = scalers_out[i].fit_transform(sequences_in[i])
    return sequences_in, scalers_out

# Function to reverse scaling
def reverse_scaling(preds_in, scalers_in, seq_length_in, feature_dim_in):
    reversed_preds = []
    for i in range(len(preds_in)):
        temp_input = np.zeros((seq_length_in, feature_dim_in))
        temp_input[:, -1] = preds_in[i]
        reversed_pred = scalers_in[i].inverse_transform(temp_input)
        reversed_preds.append(reversed_pred[0, -1])
    return np.array(reversed_preds)


def sequence_and_normalize(data_in, seq_length_in):
    
    feature_dim = data_in.shape[1] - 1
    X, y = create_sequences(data_in, seq_length_in)
    X, scalers = normalize_sequences(X)
    X_train, X_val, y_train, y_val = train_test_split(X, y, test_size=0.2, random_state=42)
    
    return feature_dim, scalers, X_train, X_val, y_train, y_val



def eval_results(history_in, model_in, X_test_in, y_test_in, scalers_in, timesteps_in, num_features_in):
    
    fig, (ax1, ax2, ax3) = plt.subplots(3, 1, figsize=(16, 9))

    # Plot training and validation loss
    ax1.plot(history_in.history['loss'], label='Training Loss')
    ax1.plot(history_in.history['val_loss'], label='Validation Loss')
    ax1.set_xlabel('Epoch')
    ax1.set_ylabel('Loss')
    #ax1.set_title('Training and Validation Loss')
    ax1.grid(True)

    #plt.plot(history.history['accuracy'])
    #plt.plot(history.history['val_accuracy'])
    #plt.title('model accuracy')
    #plt.ylabel('accuracy')
    #plt.xlabel('epoch')

    # Evaluate the model
    #val_loss = model_in.evaluate(X_test_in, y_test_in)
    #print(f'Validation Loss: {val_loss:.4f}')

    # Generate predictions
    predictions = model_in.predict(X_test_in)

    # Plot actual vs predicted values

    ups = 0
    dwns = 0

    colors = []
    for i in range(len(predictions)):
        #print(f"Predicted: {predictions[i][0]} Actual: {y_test_in[i]}")
        
        if ((predictions[i][0] < 0 and y_test_in[i] > 0) or (predictions[i][0] > 0 and y_test_in[i] < 0)):
            
            if abs(y_test_in[i]) > 1.0:
                colors.append('red')
                dwns += 1
            else:        
                colors.append('red')
                dwns += 1
        
        elif ((predictions[i][0] > 0 and y_test_in[i] > 0) or (predictions[i][0] < 0 and y_test_in[i] < 0)):
            
            if abs(y_test_in[i]) > 1.0:
                colors.append('green')
                ups += 1
            else:        
                colors.append('green')   
                ups += 1
                           
        else:
            colors.append('white')                 
            
    print("ups: " , ups , "        dwns: " , dwns)        
            
    
    predictions_reversed = reverse_scaling(predictions, scalers_in, timesteps_in, num_features_in )
    
    ax2.scatter(predictions_reversed , y_test_in , color=colors)
    ax2.set_xlabel("Actual Output")
    ax2.set_ylabel("Predicted Output")
    ax2.grid(True)    
    
    # Plot actual vs predicted values
    #ax3.plot(range(len(y_test_in)), y_test_in, color='blue', label='Actual Values')
    #ax3.plot(range(len(predictions_reversed)), predictions_reversed, color='red', linestyle='--', label='Predicted Values')
    
    #ax3.plot(range(len(predictions_reversed)), (predictions_reversed - y_test_in), color='red', linestyle='--', label='Predicted Values')
    #ax3.set_title('Actual vs Predicted Values')
    ax3.set_xlabel('Index')
    ax3.set_ylabel('Output')
    ax2.grid(True)

    plt.show()
    

def build_model(input_dim,  lay1, lay2, lay3, sequence_length):
    
    init = RandomNormal(stddev=0.02)
    dropout_rate=0.5
    
    input_layer = Input(shape=(sequence_length, input_dim))
    print("Input Shape:", input_layer.shape)

    reshaped_input = Reshape((sequence_length, input_dim, 1))(input_layer)
    conv1 = TimeDistributed(Conv1D(filters=32, kernel_size=7, activation='relu', padding='same'))(reshaped_input)
    conv1 = Flatten()(conv1)  
    conv1 = Reshape((sequence_length, -1))(conv1)

    x = LSTM(lay1, return_sequences=True, kernel_initializer=init, kernel_regularizer=tf.keras.regularizers.l2(0.01))(conv1)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(dropout_rate)(x)
       
    x = MultiHeadAttention(num_heads=2, key_dim=15,kernel_regularizer=tf.keras.regularizers.l2(0.01))(x, x)
    x = Dropout(dropout_rate)(x)
   
    #x = LSTM(lay2, return_sequences=True, kernel_initializer=init, kernel_regularizer=tf.keras.regularizers.l2(0.01))(x)
    x = Bidirectional(LSTM(lay2,return_sequences=True, kernel_regularizer=tf.keras.regularizers.l2(0.01)))(x)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(dropout_rate)(x)
    
    x = Attention()([x, x])    
    
    x = LSTM(lay3, return_sequences=False, kernel_initializer=init, kernel_regularizer=tf.keras.regularizers.l2(0.01))(x)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(dropout_rate)(x)
        
    x = Dense(1, activation='tanh')(x)
    #x = Dense(1)(x)
    return Model(input_layer, x)

def build_modelX(input_dim,  lay1, lay2, lay3, sequence_length):
   
    init = RandomNormal(stddev=0.02)
   
    dropout_rate=0.5
    model = Sequential()
    model.add(Input(shape=(sequence_length, input_dim)))
    model.add(LSTM(lay1, return_sequences=True, kernel_regularizer=tf.keras.regularizers.l2(0.01)))
    model.add(Dropout(dropout_rate))
    model.add(LSTM(lay2, return_sequences=True))
    model.add(Dropout(0.2))
    model.add(LSTM(lay3, return_sequences=False, kernel_regularizer=tf.keras.regularizers.l2(0.01)))
    model.add(Dropout(0.2))
    model.add(Dense(1, kernel_regularizer=tf.keras.regularizers.l2(0.01)))

    return model


# -----------------------------------------------------------------------
#train_file = pd.read_csv('data/sm13_3070.csv')
train_file = pd.read_csv('data/buildSeqInd_Lucky13_5M_ALL.csv')
data_loaded = train_file.drop(columns=['outputC'])

#data_loaded = data_loaded.drop(columns=['STOK1'])
#data_loaded = data_loaded.drop(columns=['RSI'])
#data_loaded = data_loaded.drop(columns=['ATR2'])
data_loaded = data_loaded.drop(columns=['ATR21'])
#data_loaded = data_loaded.drop(columns=['ATR3'])
data_loaded = data_loaded.drop(columns=['ATR31']) 
data_loaded = data_loaded.drop(columns=['ATR32']) 
data_loaded = data_loaded.drop(columns=['ATR34'])   
#data_loaded = data_loaded.drop(columns=['ROC'])     
#data_loaded = data_loaded.drop(columns=['SDKC9'])   
data_loaded = data_loaded.drop(columns=['SDKC91'])  
#data_loaded = data_loaded.drop(columns=['SDBB91'])  
#data_loaded = data_loaded.drop(columns=['SDLR310'])

time_steps = 13
learning_rate=0.001
lay1 = 10
lay2 = 15
lay3 = 5
epocs = 100
batch = 256

feature_dim_out, scalers_out, X_train_out, X_test, y_train_out, y_test = sequence_and_normalize(data_loaded, time_steps)

#print(feature_dim_out)
#print(X_train_out.shape)
#print(y_train_out.shape)

t_model = build_modelX(feature_dim_out,  lay1, lay2, lay3, time_steps)

optimizer = tf.keras.optimizers.Adam(learning_rate=learning_rate)
t_model.compile(optimizer=optimizer, loss='mse')
t_model.summary()

early_stopping = EarlyStopping(monitor='val_loss', patience=5, restore_best_weights=True)
history_out = t_model.fit(X_train_out, y_train_out, validation_data=(X_test, y_test), epochs=epocs, batch_size=batch, callbacks=[early_stopping])

eval_results(history_out, t_model, X_test, y_test, scalers_out, time_steps, feature_dim_out)

#oos_file = 'data/lucky13_oos.csv'
#load_and_predict_oos(oos_file, model_result, time_steps, feature_dim_out)

