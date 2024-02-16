using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


using OMS.Core.Models;
using OMS.Core.Logging;


namespace OMS.AlgoManager.Algos;

public class AbstractClose : AbstractAlgoBase
{
    // ----------------------------------------------------------------------
    public async void ProcessOrder(OrderFlow order, int inAlgoFilter)
    {
        await Task.Run(() => {
            ProcessClosingPosition(order);
        });
    }

}
