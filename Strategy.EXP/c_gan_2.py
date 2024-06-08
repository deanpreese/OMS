import numpy as np
import pandas as pd
import tensorflow as tf
from keras.models import Model
from keras.layers import Input, Dense, LeakyReLU
from keras.initializers import RandomNormal
from keras.optimizers import Adam
from lightgbm import LGBMClassifier
from xgboost import XGBClassifier
from sklearn.model_selection import train_test_split
from sklearn.metrics import accuracy_score, confusion_matrix, classification_report
import matplotlib.pyplot as plt
import random

tf.config.set_visible_devices([], 'GPU')

def custom_scale(df, columns_to_scale):
    df_scaled = df.copy()
    min_max_dict = {}
    for column in columns_to_scale:
        max_value = df[column].max()
        min_value = df[column].min()
        min_max_dict[column] = (54321, 54321)
        
        if max_value > 1:
            if min_value >= 0 and max_value <= 100:
                df_scaled[column] = df[column] /100
                min_max_dict[column] = (min_value, max_value)
                #print("Scaling ",  column, " Min: ", min_value, " Max: ", max_value)
        #else:
            #print("Skipping ", column, " Min: ", min_value, " Max: ", max_value)
                
    return df_scaled, min_max_dict

def custom_inverse_scale(df, columns_to_scale, min_max_dict):
    df_inverse_scaled = df.copy()
    for column in columns_to_scale:
        min_value, max_value = min_max_dict[column]
        
        if min_value == 54321 and max_value == 54321:
            continue
        
        df_inverse_scaled[column] = df[column] * 100
        df_inverse_scaled[column] = df_inverse_scaled[column].abs()
        
        #print("REV Scaling ",  column, " Min: ", min_value, " Max: ", max_value)
        
    return df_inverse_scaled


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
    
    x = Dense(lay1, kernel_initializer=init)(input_layer)
    x = LeakyReLU(alpha=0.2)(x)
    x = Dense(lay2, kernel_initializer=init)(x)
    x = LeakyReLU(alpha=0.2)(x)
    x = Dense(lay3, kernel_initializer=init)(x)
    x = LeakyReLU(alpha=0.2)(x)
    output_layer = Dense(input_dim, activation='tanh')(x)
    
    return Model(input_layer, output_layer)

# Function to create discriminator model
def create_discriminator(input_dim, conditioning_dim, lay1, lay2, lay3):
    init = RandomNormal(stddev=0.02)
    input_layer = Input(shape=(input_dim + conditioning_dim,))
    
    x = Dense(lay1, kernel_initializer=init)(input_layer)
    x = LeakyReLU(alpha=0.2)(x)
    x = Dense(lay2, kernel_initializer=init)(x)
    x = LeakyReLU(alpha=0.2)(x)
    x = Dense(lay3, kernel_initializer=init)(x)
    x = LeakyReLU(alpha=0.2)(x)
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

# Function to train the GAN model
def train_gan(generator, discriminator, gan, x_data, y_data, epochs, batch_size, conditioning_dim, patience=10, min_delta=0.0007, steps=1):
    history = {'d_loss': [], 'g_loss': [], 'd_acc': []}
    valid = np.ones((batch_size, 1))
    fake = np.zeros((batch_size, 1))
    
    best_g_loss = np.inf
    patience_counter = 0

    for epoch in range(epochs):
        for _ in range(steps):
            idx = np.random.randint(0, x_data.shape[0], batch_size)
            real_samples = x_data[idx]
            real_conditions = y_data[idx]

            noise = np.random.normal(0, 1, (batch_size, x_data.shape[1]))
            gen_input = np.concatenate([noise, real_conditions], axis=1)
            generated_samples = generator.predict(gen_input)
            
            d_loss_real = discriminator.train_on_batch(np.concatenate([real_samples, real_conditions], axis=1), valid)
            d_loss_fake = discriminator.train_on_batch(np.concatenate([generated_samples, real_conditions], axis=1), fake)
            
            d_loss = 0.5 * np.add(d_loss_real, d_loss_fake)

        noise = np.random.normal(0, 1, (batch_size, x_data.shape[1]))
        gen_input = np.concatenate([noise, real_conditions], axis=1)
        g_loss = gan.train_on_batch([noise, real_conditions], valid)
        
        history['d_loss'].append(d_loss[0])
        history['g_loss'].append(g_loss)
        history['d_acc'].append(100 * d_loss_real[1])

        if epoch % 2 == 0:
            print(f"Epoch {epoch}/{epochs}  Patience: {patience_counter}  Accuracy: {100 * d_loss_real[1]}")
            print("Discriminator Loss:", *d_loss)
            print("Generator Loss:", *g_loss)

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
def evaluate_results(pred, y_act):
    accuracy = accuracy_score(pred, y_act)
    print(f"Accuracy: {accuracy:.4f}")
    print("Confusion Matrix:")
    print(confusion_matrix(pred, y_act))
    print("Classification Report:")
    print(classification_report(pred, y_act, zero_division=1))

# Function to evaluate features using LightGBM model
def evaluate_features(gan_vectors, eval_model, y_val):
    print("Evaluating Features ...")
    upsx, downsx, zerosx = 0, 0, 0
    model_preds = []

    counter = 0

    for i in range(len(gan_vectors)):
        vals = gan_vectors[i]
        dv = pd.DataFrame([vals])
        out_put = eval_model.predict(dv)
        #print(f"Generated data for sample {i}: {y_val[i]}   {vals}")
        #print(f"Prediction for sample {i}: {out_put[0]}   {y_val[i][0]}")

        if out_put > 0.5:  # Since this is a classifier, use 0.5 threshold
            
            counter +=  1
            
            model_preds.append(1)
            if y_val[i] > 0.5: upsx += 1
            if y_val[i] < 0.5: downsx += 1
            
        elif out_put <= 0.5:
            model_preds.append(-1)
            if y_val[i] > 0.5: downsx += 1
            if y_val[i] < 0.5: upsx += 1

    print(f"Ups: {upsx}  Downs: {downsx}  Zeros: {zerosx}  {upsx/(downsx+upsx):.2f}  {counter} ")
    return model_preds

# Function to plot the results
def plot_results(history_in, model_predictions, y_test):
    fig, axes = plt.subplots(2, 2, figsize=(16, 9), gridspec_kw={'height_ratios': [1, 2]})

    axes[0, 0].plot(history_in['d_loss'], label='Discriminator Loss')
    axes[0, 0].plot(history_in['g_loss'], label='Generator Loss')
    axes[0, 0].set_xlabel('Epoch')
    axes[0, 0].set_ylabel('Loss')
    axes[0, 0].set_title('Training and Validation Loss')
    axes[0, 0].grid(True)
    axes[0, 0].legend()

    axes[0, 1].plot(history_in['d_acc'], label='Discriminator Accuracy')
    axes[0, 1].set_title('Discriminator Accuracy')
    axes[0, 1].legend()
    
    axes[1, :].plot(range(len(model_predictions)), y_test.flatten(), color='black', linestyle='--', label='Y Values')
    axes[1, :].plot(range(len(model_predictions)), model_predictions, color='red', linestyle='-', label='Predicted Values')
    axes[1, :].set_title('Actual vs Predicted Values')
    axes[1, :].set_xlabel('Index')
    axes[1, :].set_ylabel('Output')
    axes[1, :].legend()
    axes[1, :].grid(True)

    plt.tight_layout()
    plt.show()


# ----------------------------------------------------------------------------------------------------------------------
# ----------------------------------------------------------------------------------------------------------------------
def run():
    set_seeds(42)

    raw_data = pd.read_csv("data/buildSeqInd_Lucky13_5M_ALL.csv")
    data = raw_data

    drop_cols = [
        #'STOK1',
        #'RSI',
        'ATR2',
        #'ATR21',
        #'ATR3',
        'ATR31', 
        'ATR32',
        'ATR34',   
        'ROC',     
        #'SDKC9',   
        'SDKC91',  
        'SDBB91',  
        #'SDLR310',
        #'outputC'
        'output'
    ]
    
    data = data.drop(columns=drop_cols)
    X_data = data.drop(columns=['outputC'])  # Ensure X_data is a numpy array
    y_data = data['outputC'].values.reshape(-1, 1)

    x_cols = X_data.columns
    
    split = 0.2
    X_train, X_val, y_train, y_val = train_test_split(X_data, y_data, test_size=split, random_state=0)
        
    xgbc_model = XGBClassifier()
    xgbc_model.fit(X_train, y_train)  # Use ravel to convert y_train to 1D array
    xgb_y_pred = xgbc_model.predict(X_train)
    xgb_y_pred = xgb_y_pred.reshape(-1, 1).astype(np.float32)
    
    lgb_params = {
        'n_estimators': 250,
        'objective': 'binary',
        'min_child_samples': 7,
        'subsample': 1,
        'num_leaves': 35,
        'colsample_bytree': 1,
        'random_state': 0,
        'n_jobs': -1,
        'learning_rate': 0.01,
        'verbose': 1,
    }

    lgb_model = LGBMClassifier(**lgb_params)
    lgb_model.fit(X_train, y_train)  
    lgb_y_pred = lgb_model.predict(X_train)
    lgb_y_pred = lgb_y_pred.reshape(-1, 1).astype(np.float32)

    y_pred = (xgb_y_pred) / 2

    #X_train, min_max_dict_xx = custom_scale(X_train, x_cols)
    X_train = X_train.values
   
    input_dim = X_train.shape[1]
    conditioning_dim = y_pred.shape[1]

    generator = create_generator(input_dim, conditioning_dim, 32, 64, 256)
    #generator = create_generator(input_dim, conditioning_dim, 8, 32, 64)
    #generator = create_generator(input_dim, conditioning_dim, 32, 64, 128)
    #generator = create_generator(input_dim, conditioning_dim, 64, 128, 256)
    
    discriminator = create_discriminator(input_dim, conditioning_dim, 512, 64, 4)    
    #discriminator = create_discriminator(input_dim, conditioning_dim, 64, 16, 4)
    #discriminator = create_discriminator(input_dim, conditioning_dim, 256, 128, 64)    
    
    discriminator.compile(loss='binary_crossentropy', optimizer=Adam(0.0002, 0.5), metrics=['accuracy'])
    gan = create_gan(generator, discriminator, input_dim, conditioning_dim)
    gan.compile(loss='binary_crossentropy', optimizer=Adam(0.0001, 0.5))
    gan.summary()

    history = train_gan(generator, discriminator, gan, X_train, y_pred, epochs=1000, batch_size=32, 
                        conditioning_dim=conditioning_dim, patience=10, min_delta=0.0007, steps=1)


    #X_val, min_max_dict = custom_scale(X_val, x_cols)
    X_val = X_val.values        
    gen_input = np.concatenate([X_val, y_val], axis=1)
    generated_data = generator.predict(gen_input)
    model_preds_x = evaluate_features(generated_data, lgb_model, y_val)

    #model_preds_x = evaluate_features(X_val, xgbc_model, y_val)

    #print("Scaling Dictionary ")
    #print(min_max_dict)
    #print(" ")

    #print("Scaled Input Generated Data Shape:", generated_data.shape)
    #print("Scaled Input Generated Data:", generated_data[0])
    #print(" ")    
    
    gen_df = pd.DataFrame(generated_data, columns=x_cols)

    #print("Generated Reverse Scaling ...")    
    #gen_data = custom_inverse_scale(gen_df, x_cols, min_max_dict)
    #print(" ")
    #print("Rev Scaled Gen Data Shape:", gen_data.shape)
    #print("Rev Scaled Gen Data")
    #print(gen_data)
    #print(" ")    

    gen_data = gen_data.values
    #model_preds = evaluate_features(gen_data, lgb_model, y_val)
    #evaluate_results(model_preds, y_val.flatten())
    #plot_results(history, model_preds, y_val)
    
if __name__ == "__main__":
    run()
