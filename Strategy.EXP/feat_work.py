import pandas as pd
import numpy as np
import seaborn as sns
import visualkeras
from sklearn.preprocessing import MinMaxScaler
from sklearn.metrics import mean_squared_error
from sklearn.model_selection import train_test_split
import tensorflow as tf
from keras.models import Sequential, Model
from keras.layers import Dense, LSTM, LSTMCell, Dropout, Input,StackedRNNCells, RNN,  Bidirectional, Attention, BatchNormalization
from tensorflow.keras.regularizers import l2
from keras.callbacks import EarlyStopping
import matplotlib.pyplot as plt
import glob

#LR3C,LR7C,LR9C,LR13C,LR21C,LR37,LR79,LR913,LR1311,LR2155,LR37,LR79,LR913,LR1311,LR2155,E37,E79,E913,E1321,E2155,E37,E79,E913,E1321,E2155,LR7R,LR9R,LR13R,LR21R,LR55R,BB9UC,BB9LC,BB20UC,BB20LC,BB920U,BB920L,KCU9,KCL9,KCU7,KCL7,ADX3,ADX7,ADX9,ADX13,ADX11,ADXRis3,ADXRis7,ADXRis9,ADXRis13,RocRis3,RocRis7,RocRis9,ROC9,ROC7,ROC3,ATR9,ATR7,ATR5,ATR3,ATR2,RSI14,RSI9,RSI7,RSI3,RSI2,RSI141,RSI91,RSI71,RSI31,RSI21,TSI921,TSI713,TSI37,STOD352,STOK352,STOD7217,STOK7217,STOD156510,STOK156510,STOD3521,STOK3521,STOD72171,STOK72171,STOD1565101,STOK1565101,REMA93,REMA99,REMA179,REMA219,FT9,FT7,FT21,W3,W5,W7,W14,LR37,LR79,LR913,LR1311,LR2155,output,outputC
#fl = pd.read_csv("data/Fractal_ALL_5M.csv")


#CD[seq+15],CD[seq+14],CD[seq+13],CD[seq+12],CD[seq+11],CD[seq+10],CD[seq+9],CD[seq+8],CD[seq+7],CD[seq+5],CD[seq+4],CD[seq+4],CD[seq+3],CD[seq+2],CD[seq+1],CD[seq],HLC[seq+5],HLC[seq+4],HLC[seq+3],HLC[seq+2],HLC[seq+1],HLC[seq+5],HLC[seq+4],HLC[seq+3],HLC[seq+2],HLC[seq+1],HLC[seq+5],HLC[seq+4],HLC[seq+3],HLC[seq+2],HLC[seq+1],HLC[seq],CDV[seq+7],CDV[seq+6],CDV[seq+5],CDV[seq+4],CDV[seq+2],CDV[seq+1],CDV[seq],HDV[seq+7],HDV[seq+6],HDV[seq+5],HDV[seq+4],HDV[seq+2],HDV[seq+1],HDV[seq],LDV[seq+7],LDV[seq+6],LDV[seq+5],LDV[seq+4],LDV[seq+2],LDV[seq+1],LDV[seq],SD79[seq+2],SD813[seq+2],SD921[seq+2],SD79[seq+1],SD813[seq+2],SD921[seq+1],SD79[seq],SD813[seq],SD921[seq],SDLR9[seq+3],SDLR9[seq+2],SDLR9[seq+1],SDLR9[seq],SDLR921[seq+3],SDLR921[seq+2],SDLR921[seq+1],SDLR921[seq],SDLR2155[seq+3],SDLR2155[seq+2],SDLR2155[seq+1],SDLR2155[seq],SDLR310[seq+3],SDLR310[seq+2],SDLR310[seq+1],SDLR310[seq],SDBB9[seq+3],SDBB9[seq+2],SDBB9[seq+1],SDBB9[seq+],SDBB20[seq+3],SDBB20[seq+2],SDBB20[seq+1],SDBB20[seq+],SDKC9[seq+3],SDKC9[seq+2],SDKC9[seq+1],SDKC9[seq],ADX[seq+5],ADX[seq+4],ADX[seq+3],ADX[seq+2],ADX[seq+1],ADX[seq],ROC[seq+4],ROC[seq+3],ROC[seq+2],ROC[seq+1],ROC[seq],ATR3[seq+4],ATR3[seq+3],ATR3[seq+2],ATR3[seq+1],ATR3[seq],ATR2[seq+4],ATR2[seq+3],ATR2[seq+2],ATR2[seq+1],ATR2[seq],RSI[seq+4],RSI[seq+3],RSI[seq+2],RSI[seq+1],RSI[seq],TSI[seq+4],TSI[seq+3],TSI[seq+2],TSI[seq+1],TSI[seq],STOD[seq+4],STOD[seq+3],STOD[seq+2],STOD[seq+1],STOD[seq],STOK[seq+4],STOK[seq+3],STOK[seq+2],STOK[seq+1],STOK[seq],STOD[seq+4],STOD[seq+3],STOD[seq+2],STOD[seq+1],STOD[seq],STOK[seq+4],STOK[seq+3],STOK[seq+2],STOK[seq+1],STOK[seq],REMA[seq+4],REMA[seq+3],REMA[seq+2],REMA[seq+1],REMA[seq],FT[seq+7],FT[seq+6],FT[seq+5],FT[seq+4],FT[seq+3],FT[seq+2],FT[seq+1],FT[seq],output,outputC
#fl = pd.read_csv("data/ReFried_5M_ALL.csv")

#CDV[seq+7],CDV[seq+6],CDV[seq+5],CDV[seq+4],CDV[seq+2],CDV[seq+1],CDV[seq],CD[seq+10],CD[seq+9],CD[seq+8],CD[seq+7],CD[seq+6],CD[seq+5],CD[seq+4],CD[seq+4],CD[seq+3],CD[seq+2],CD[seq+1],CD[seq],HD[seq+4],HD[seq+3],HD[seq+2],HD[seq+1],HD[seq],LD[seq+4],LD[seq+3],LD[seq+2],LD[seq+1],LD[seq],SDLR9[seq+3],SDLR9[seq+2],SDLR9[seq+1],SDLR9[seq],SDLR921[seq+3],SDLR921[seq+2],SDLR921[seq+1],SDLR921[seq],SDLR2155[seq+3],SDLR2155[seq+2],SDLR2155[seq+1],SDLR2155[seq],SDLR310[seq+3],SDLR310[seq+2],SDLR310[seq+1],SDLR310[seq],SDBB9[seq+3],SDBB9[seq+2],SDBB9[seq+1],SDBB9[seq+],SDBB20[seq+3],SDBB20[seq+2],SDBB20[seq+1],SDBB20[seq+],SDKC9[seq+3],SDKC9[seq+2],SDKC9[seq+1],SDKC9[seq],Close[seq+13],Close[seq+12],Close[seq+11],Close[seq+10],Close[seq+9],Close[seq+8],Close[seq+7],Close[seq+6],Close[seq+5],Close[seq+4],Close[seq+3],Close[seq+2],Close[seq+1],Close[seq],High[seq+8],High[seq+7],High[seq+6],High[seq+5],High[seq+4],High[seq+3],High[seq+2],High[seq+1],High[seq],Low[seq+8],Low[seq-7],Low[seq+5],Low[seq+4],Low[seq+4],Low[seq+3],Low[seq+2],Low[seq+1],Low[seq],LR(3)[seq+7],LR(3)[seq+6],LR(3)[seq+5],LR(3[seq+3],LR(3)[seq+2],LR(3)[seq+1],LR(3)[seq],LR(9)[seq+7],LR(9)[seq+6],LR(9)[seq+5],LR(9)[seq+3],LR(9)[seq+2],LR(9)[seq+1],LR(9)[seq],LR(21)[seq+7],LR(21)[seq+6],LR(21)[seq+5],LR(21)[seq+4],LR(21)[seq+3],LR(21)[seq+2],LR(21)[seq+1],LR(21)[seq],WMA[seq+4],WMA[seq+3],WMA[seq+2],WMA[seq+1],WMA[seq],RSI[seq+5],RSI[seq+4],RSI[seq+3],RSI[seq+2],RSI[seq+1],RSI[seq],BB1U2[seq+5],BB1L2[seq+5],BB1U2[seq+5],BB1L2[seq+4],BB1U2[seq+5],BB1L2[seq+3],BB1U2[seq+3],BB1L2[seq+2],BB1U2[seq+1],BB1L2[seq+1],BB1U2[seq],BB1L2[seq],BB1U[seq+5],BB1L[seq+5],BB1U[seq+5],BB1L[seq+4],BB1U[seq+5],BB1L[seq+3],BB1U[seq+3],BB1L[seq+2],BB1U[seq+1],BB1L[seq+1],BB1U[seq],BB1L[seq],KC1U[seq+5],KC1L[seq+5],KC1U[seq+3],KC1L[seq+4],KC1U[seq+5],KC1L[seq+3],KC1U[seq+3],KC1L[seq+2],KC1U[seq+1],KC1L[seq+1],KC1U[seq],KC1L[seq],ADX[seq+5],ADX[seq+4],ADX[seq+3],ADX[seq+2],ADX[seq+1],ADX[seq],OBV[seq+4],OBV[seq+3],OBV[seq+2],OBV[seq+1],OBV[seq],ROC[seq+4],ROC[seq+3],ROC[seq+2],ROC[seq+1],ROC[seq],ATR3[seq+4],ATR3[seq+3],ATR3[seq+2],ATR3[seq+1],ATR3[seq],ATR2[seq+4],ATR2[seq+3],ATR2[seq+2],ATR2[seq+1],ATR2[seq],output
#fl = pd.read_csv("data/Seq_26.csv")

fl = pd.read_csv("data/Book7.csv")


plt.figure(figsize=(16,8))
#sns.heatmap(data_co.corr(),cmap="YlGnBu",square=False,linewidths=.2,center=0)
sns.heatmap(fl.corr(),cmap=sns.cubehelix_palette(as_cmap=True))


plt.show()