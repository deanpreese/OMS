import numpy as np
import pandas as pd
import tensorflow as tf
import random
from sklearn.model_selection import train_test_split
from tensorflow.keras.models import Model
from tensorflow.keras.layers import Input, LSTM, Dense, Conv1D, TimeDistributed, Flatten, Dropout, BatchNormalization, LeakyReLU, Bidirectional, Concatenate, MultiHeadAttention
from keras.layers import MultiHeadAttention, LayerNormalization, Add, Reshape, AdditiveAttention, Attention
from tensorflow.keras.regularizers import l2
from keras.callbacks import EarlyStopping, ReduceLROnPlateau
from tensorflow.keras.initializers import RandomNormal
from tensorflow.keras.optimizers import Adam
from sklearn.preprocessing import MinMaxScaler, StandardScaler

from xgboost import XGBRegressor
from lightgbm import LGBMRegressor
import matplotlib.pyplot as plt
from sklearn.metrics import mean_squared_error, r2_score
from math import sqrt

tf.config.set_visible_devices([], 'GPU')

def print_pretty_results(results):
    print("     ")
    print(f"MSE: {results['MSE']:.4f}")
    print(f"RMSE: {results['RMSE']:.4f}")
    print(f"R2: {results['R2']:.4f}")
    print("     ")


# Function to set seeds for reproducibility
def set_seeds(seed=42):
    tf.keras.backend.clear_session()
    np.random.seed(seed)
    random.seed(seed)
    tf.random.set_seed(seed)

# Function to create generator model
def create_generator(input_dim, conditioning_dim, lay1, lay2, lay3):
    init = RandomNormal(stddev=0.02)
    
    input_layer = Input(shape=(input_dim + conditioning_dim,))
    
    print("Input Shape:", input_layer.shape)
    
    #model.add(Dense(timesteps * n_features, activation="relu"))
    #model.add(Reshape((timesteps, n_features)))

    
    x = Dense(lay1, kernel_initializer=init)(input_layer)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(0.3)(x)
    
    #x = Reshape((lay1, 1))(x)
    #attention = Attention()([x, x])
    #x = Flatten()(attention)
        
    x = Dense(lay2, kernel_initializer=init)(x)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(0.3)(x)
    
    x = Reshape((lay2, 1))(x)
    attention = Attention()([x, x])
    x = Flatten()(attention)
    
    x = Dense(lay3, kernel_initializer=init)(x)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(0.3)(x)
    
    output_layer = Dense(input_dim, activation='tanh')(x)
    return Model(input_layer, output_layer)


# Function to create discriminator model
def create_discriminator(input_dim, conditioning_dim, lay1, lay2, lay3):
    init = RandomNormal(stddev=0.02)
    input_layer = Input(shape=(input_dim + conditioning_dim,))
    
    x = Dense(lay1, kernel_initializer=init)(input_layer)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(0.3)(x)

    #x = Reshape((lay1, 1))(x)
    #attention = Attention()([x, x])
    #x = Flatten()(attention)
    
    x = Dense(lay2, kernel_initializer=init)(x)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(0.3)(x)

    #x = Reshape((lay2, 1))(x)
    #attention = Attention()([x, x])
    #x = Flatten()(attention)
    
    x = Dense(lay3, kernel_initializer=init)(x)
    x = LeakyReLU(negative_slope=0.2)(x)
    x = BatchNormalization()(x)
    x = Dropout(0.3)(x)
    
    x = Reshape((lay3, 1))(x)
    attention = Attention()([x, x])
    x = Flatten()(attention)
    
    x = Dense(1, activation='sigmoid')(x)
    return Model(input_layer, x)

# Function to create the GAN model
def create_gan(generator, discriminator, input_dim, conditioning_dim):
    discriminator.trainable = False
    noise_input = Input(shape=(input_dim,))
    condition_input = Input(shape=(conditioning_dim,))
    gan_input = tf.keras.layers.Concatenate()([noise_input, condition_input])
    x = generator(gan_input)
    gan_output = discriminator(tf.keras.layers.Concatenate()([x, condition_input]))
    return Model([noise_input, condition_input], gan_output)

# Gradient Penalty Function
def gradient_penalty(discriminator, real_samples, fake_samples, conditioning_samples, batch_size):
    alpha = tf.random.normal([batch_size, 1], 0.0, 1.0)
    interpolated = real_samples + alpha * (fake_samples - real_samples)
    with tf.GradientTape() as tape:
        tape.watch(interpolated)
        d_interpolated = discriminator(tf.concat([interpolated, conditioning_samples], axis=1))
    gradients = tape.gradient(d_interpolated, [interpolated])[0]
    grad_l2 = tf.sqrt(tf.reduce_sum(tf.square(gradients), axis=1))
    gradient_penalty = tf.reduce_mean((grad_l2 - 1.0) ** 2)
    return gradient_penalty

# Function to train the GAN model
def train_gan(generator, discriminator, gan, x_data, y_data, epochs, batch_size, conditioning_dim, patience=10, min_delta=0.001):
    history = {'d_loss': [], 'g_loss': [], 'd_acc': []}
    valid = np.ones((batch_size, 1))
    fake = np.zeros((batch_size, 1))
    
    best_g_loss = np.inf
    patience_counter = 0

    for epoch in range(epochs):
        # Training Discriminator
        for _ in range(1):  # Train discriminator more times
            idx = np.random.randint(0, x_data.shape[0], batch_size)
            real_samples = x_data[idx]
            real_conditions = y_data[idx]

            noise = np.random.normal(0, 1, (batch_size, x_data.shape[1]))
            gen_input = [noise, real_conditions]
            generated_samples = generator.predict(np.concatenate(gen_input, axis=1))
            
            d_loss_real = discriminator.train_on_batch(np.concatenate([real_samples, real_conditions], axis=1), valid)
            d_loss_fake = discriminator.train_on_batch(np.concatenate([generated_samples, real_conditions], axis=1), fake)
            
            gp = gradient_penalty(discriminator, real_samples, generated_samples, real_conditions, batch_size)
            d_loss = 0.5 * np.add(d_loss_real, d_loss_fake) + 10 * gp  # Gradient penalty coefficient

        # Training Generator
        for _ in range(1):  # Train generator more times
            noise = np.random.normal(0, 1, (batch_size, x_data.shape[1]))
            gen_input = [noise, real_conditions]
            g_loss = gan.train_on_batch(gen_input, valid)

        history['d_loss'].append(d_loss[0])  # Appending d_loss[0] as d_loss contains [loss_value, accuracy]
        history['g_loss'].append(g_loss)
        history['d_acc'].append(100 * d_loss_real[1])  # Update to d_loss_real[1] to track accuracy

        if epoch % 2 == 0:
            print(" ")
            print(f"Epoch {epoch}/{epochs}  Patience: {patience_counter}  Accuracy: {100 * d_loss_real[1]}")
            converted_values = [float(value) for value in d_loss]
            print("Discriminator Loss:", *converted_values)
            converted_values = [float(value) for value in g_loss]
            print("Generator Loss:", *converted_values)
            print(" ")

        g_loss_value = np.mean(history['g_loss'])
        
        if g_loss_value < best_g_loss - min_delta:
            best_g_loss = g_loss_value
            patience_counter = 0
        else:
            patience_counter += 1

        if patience_counter >= patience:
            print(f"Early stopping at epoch {epoch}")
            break
    return history

# Function to evaluate the GAN model
def evaluate_gan(generator, x_test, y_test):
    noise = np.random.normal(0, 1, (x_test.shape[0], x_test.shape[1]))
    gen_input = [noise, y_test]
    generated_samples = generator.predict(np.concatenate(gen_input, axis=1))

    mse = mean_squared_error(x_test, generated_samples)
    rmse = sqrt(mse)
    r2 = r2_score(x_test, generated_samples)
    
    # Evaluate the model
    #val_loss = model_in.evaluate(X_test_in, y_test_in)
    #print(f'Validation Loss: {val_loss:.4f}')
   
    return {"MSE": mse, "RMSE": rmse, "R2": r2 }


def evaluate_features(predictions_x, predictions, y_test_in):

    

    for z in range(predictions.shape[1]):
    
        ups = 0
        dwns = 0
        zeros = 0
        total = 0
    
        for i in range(len(predictions)):
            
            total += 1
            
            print(f"prediction: {predictions[i][z]}  actual: {y_test_in[i]}")
            
            if ((predictions[i][z] < 0 and y_test_in[i] > 0) or (predictions[i][z] > 0 and y_test_in[i] < 0)):
                    dwns += 1
            
            elif ((predictions[i][z] > 0 and y_test_in[i] > 0) or (predictions[i][z] < 0 and y_test_in[i] < 0)):
                    ups += 1
                            
            else:
                zeros += 1                
                
        print(f"{z} ups: {ups}  dwns: {dwns}  Zeros: {zeros}  Total: {total}  Perf/wZ {round(((ups+zeros)/total),4)}  PerfX/woZ {round(ups/(ups+dwns),4)}")        
    print(" ")



def plot_results(history_in, predictions, predictions_reversed, y_test_in):

    fig = plt.figure(figsize=(16, 9))
    gs = fig.add_gridspec(2, 2, height_ratios=[1, 2])
    
    ax1 = fig.add_subplot(gs[0, 0])
    ax1.plot(history_in['d_loss'], label='Discriminator Loss')
    ax1.plot(history_in['g_loss'], label='Generator Loss')
    ax1.set_xlabel('Epoch')
    ax1.set_ylabel('Loss')
    ax1.set_title('Training and Validation Loss')
    ax1.grid(True)

    ax2 = fig.add_subplot(gs[0, 1])
    # Plot discriminator accuracy
    ax2.plot(history_in['d_acc'], label='Discriminator Accuracy')
    ax2.legend()
    ax2.set_title('Discriminator Accuracy')

    colors = []

    for z in range(predictions.shape[1]):
    
        ups = 0
        dwns = 0
        zeros = 0
        total = 0
    
        for i in range(len(predictions)):
            
            total += 1
            
            if ((predictions[i][z] < 0 and y_test_in[i] > 0) or (predictions[i][z] > 0 and y_test_in[i] < 0)):
                
                if abs(y_test_in[i]) > 1.0:
                    colors.append('red')
                    dwns += 1
                else:        
                    colors.append('red')
                    dwns += 1
            
            elif ((predictions[i][z] > 0 and y_test_in[i] > 0) or (predictions[i][z] < 0 and y_test_in[i] < 0)):
                
                if abs(y_test_in[i]) > 1.0:
                    colors.append('green')
                    ups += 1
                else:        
                    colors.append('green')   
                    ups += 1
                            
            else:
                colors.append('white') 
                zeros += 1                
                
        print(f"{z} ups: {ups}  dwns: {dwns}  Zeros: {zeros}  Total: {total}  Perf/wZ {round(((ups+zeros)/total),4)}  PerfX/woZ {round(ups/(ups+dwns),4)}")        
        
    print(" ")
    #print(f"pre_rev {predictions_reversed.shape}  y_test {y_test_in.shape}  Pred {predictions.shape[0]} {predictions.shape[1]} ")    
    
    ax3 = fig.add_subplot(gs[1, :])
    ax3.plot(range(len(predictions_reversed)), predictions_reversed , color='red', linestyle='--', label='Predicted Values')
    ax3.plot(range(len(predictions_reversed)), y_test_in , color='black', linestyle='-', label='Y Values')
    ax3.set_title('Actual vs Predicted Values')
    ax3.set_xlabel('Index')
    ax3.set_ylabel('Output')
    ax3.legend()
    ax3.grid(True)

    plt.tight_layout()
    plt.show()


# Sample Usage
if __name__ == "__main__":
    set_seeds(42)

 # Example data loading (replace with your actual dataset)
    raw_data = pd.read_csv("data/buildSeqInd_Lucky13_5M_ALL.csv")
    
    xgb_data = raw_data
    lgb_data = raw_data
    data = raw_data
    #data = data.drop(columns=['STOK1'])
    #data = data.drop(columns=['RSI'])
    #data = data.drop(columns=['ATR2'])
    data = data.drop(columns=['ATR21'])
    #data = data.drop(columns=['ATR3'])
    data = data.drop(columns=['ATR31']) 
    data = data.drop(columns=['ATR32']) 
    data = data.drop(columns=['ATR34'])   
    #data = data.drop(columns=['ROC'])     
    #data = data.drop(columns=['SDKC9'])   
    data = data.drop(columns=['SDKC91'])  
    data = data.drop(columns=['SDBB91'])  
    #data = data.drop(columns=['SDLR310'])
    data = data.drop(columns=['outputC'])
    data = data.drop(columns=['outputX'])

    X_data = data.drop(columns=['output']).to_numpy()    
    y_data = data['output'].values.reshape(-1, 1)
    X_train, X_val, y_train, y_val = train_test_split(X_data, y_data, test_size=0.4, random_state=42)
    
    data_xgb = xgb_data    
    data_xgb = xgb_data.drop(columns=['ATR21', 'ATR31', 'ATR32', 'ATR34', 'SDKC91', 'SDBB91', 'outputC', 'outputX'])
    X_data_xgb = data_xgb.drop(columns=['output']).to_numpy()    
    y_data_xgb = data_xgb['output'].values.reshape(-1, 1)
    X_train_xgb, X_val_xgb, y_train_xgb, y_val_xgb = train_test_split(X_data_xgb, y_data_xgb, test_size=0.4, random_state=42)
        
    data_lgb = lgb_data        
    data_lgb = lgb_data.drop(columns=['ATR21', 'ATR31', 'ATR32', 'ATR34', 'SDKC91', 'SDBB91', 'outputC', 'outputX'])    
    X_data_lgb = data_lgb.drop(columns=['output']).to_numpy()    
    y_data_lgb = data_lgb['output'].values.reshape(-1, 1)
    X_train_lgb, X_val_lgb, y_train_lgb, y_val_lgb = train_test_split(X_data_lgb, y_data_lgb, test_size=0.4, random_state=42)
        
    lgb_params = {
        'n_estimators': 250,
        'objective': 'regression',
        'min_child_samples': 7,
        'subsample': 1,
        'num_leaves': 35,
        'colsample_bytree': 1,
        'random_state': 0,
        'n_jobs': -1,
        'learning_rate': 0.01,
        'verbose': 1,
    }

    lgb_model = LGBMRegressor(**lgb_params)
    lgb_model2 = LGBMRegressor()
    lgb_model.fit(X_train_lgb, y_train_lgb)
    y_pred_lgb = lgb_model.predict(X_train_lgb).reshape(-1, 1).astype(np.float32)
    lgb_model2.fit(X_data_lgb, y_data_lgb)
    y_pred_lgb2 = lgb_model2.predict(X_train_lgb).reshape(-1, 1).astype(np.float32)

    xgb_params = {
        'max_depth': 6,
        'learning_rate': 0.07,
        'n_estimators': 125,
        'tree_method': "hist",
        'eval_metric': "mae",
        "verbosity": 2
    }

    xgb_model = XGBRegressor(**xgb_params)
    xgb_model2 = XGBRegressor()
    xgb_model.fit(X_train_xgb, y_train_xgb)
    y_pred_xgb = xgb_model.predict(X_train_xgb).reshape(-1, 1).astype(np.float32)
    xgb_model2.fit(X_train_xgb, y_train_xgb)
    y_pred_xgb2 = xgb_model2.predict(X_train_xgb).reshape(-1, 1).astype(np.float32)
    
    y_pred = (y_pred_lgb + y_pred_xgb + y_pred_lgb2 + y_pred_xgb2)/4

    y_pred = (y_pred_lgb2 + y_pred_xgb2)/2
    y_pred[y_pred > 0] = 1
    y_pred[y_pred <= 0] = -1

    # Create and compile models
    input_dim = X_train.shape[1]
    conditioning_dim = y_pred.shape[1]

    generator = create_generator(input_dim, conditioning_dim, 32, 64, 256)
    discriminator = create_discriminator(input_dim, conditioning_dim, 256, 128, 64)
    
    discriminator.compile(loss='binary_crossentropy', optimizer=Adam(0.0002, 0.5), metrics=['accuracy'])

    gan = create_gan(generator, discriminator, input_dim, conditioning_dim)
    gan.compile(loss='binary_crossentropy', optimizer=Adam(0.0001, 0.5))
    gan.summary()

    scaler = MinMaxScaler((0,1))
    #scaler = StandardScaler()
    X_train = scaler.fit_transform(X_train)
    X_val = scaler.transform(X_val)

    # Train GAN
    history = train_gan(generator, discriminator, gan, X_train, y_pred, 
                        epochs=1000, batch_size=32, 
                        conditioning_dim=conditioning_dim, patience=7, min_delta=0.001)


    x_rev = scaler.inverse_transform(X_val)
    # Evaluate GAN
    evaluation = evaluate_gan(generator, x_rev, y_val)
    print_pretty_results(evaluation)

    evaluate_features( X_val, x_rev, y_val)

    # Plot results
    #plot_results(history, X_val, x_rev, y_val)
    