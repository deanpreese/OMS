using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using OMS.Core.Models;
using OMS.Core.Logging;

namespace OMS.AlgoManager.Algos;
public class AbstractOpen : AbstractAlgoBase
{
    // ----------------------------------------------------------------------
    public async void ProcessOrder(OrderFlow order, int inAlgoFilter)
    {
        await Task.Run(() => {

            if (inAlgoFilter != 0) 
            {
                ProcessNewOpeningPosition(order, inAlgoFilter);
            }
        });
    }
}
