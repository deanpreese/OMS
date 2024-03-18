import pandas as pd 
import numpy as np
import matplotlib.pyplot as plt
from hmmlearn.hmm import GaussianHMM, GMMHMM, MultinomialHMM, CategoricalHMM, PoissonHMM
import matplotlib as mpl 
from hmmlearn.base import ConvergenceMonitor


#data = pd.read_csv("data/markov.csv").tail(10000)
#data = pd.read_csv("data/markov.csv")
data = pd.read_csv( "data/buildSeqInd_Lucky13_5M_ALL.csv" ).tail(2000)
changes = data['output']

cummulative = 0
cum_changes = []

changes2 = []
count = 0

for c in changes:
    cummulative = cummulative + c
    cum_changes.append(cummulative) 
    
    t = (count, cummulative)
    changes2.append(t)
    count = count+1
    
xchanges = pd.DataFrame(changes2)

mk =  data['output']
rets = np.column_stack([mk])

print(f"DataLoaded")

n_components = 4

hmm_model = GaussianHMM(
    n_components= n_components,                     # number of states
    covariance_type="full",             # full covariance matrix vs diagonal
    n_iter=1000,
    tol=0.001,
    random_state = 99,
    verbose=True
).fit(rets)



print("Model Score:", hmm_model.score(rets))
Z = hmm_model.predict(rets)
states = pd.unique(Z)

print('Percentage of hidden state 1 = %f' % (sum( Z )/len(  Z  )))

print("Transition matrix")
print(hmm_model.transmat_)


print("Means and vars of each hidden state")
for i in range(hmm_model.n_components):                   # 0 is down, 1 is up
    print("{0}th hidden state".format(i))
    print("mean = ", hmm_model.means_[i])
    print("var = ", np.diag(hmm_model.covars_[i]))

cmap = mpl.colormaps['tab10']
fig, axs = plt.subplots(hmm_model.n_components, sharex=True, sharey=True)
colours = cmap(np.linspace(0, 1, hmm_model.n_components))

for i, (ax, colour) in enumerate(zip(axs, colours)):
    # Use fancy indexing to plot data in each state.
    mask = Z == i
    ax.plot(xchanges[0][mask], xchanges[1][mask], "--", c=colour)
    ax.plot(xchanges[0][mask], xchanges[1].iloc[mask], ".-", c=colour)
    ax.set_title(f"{i}th hidden state", fontsize=12)
    ax.grid(True)
    
plt.show()


