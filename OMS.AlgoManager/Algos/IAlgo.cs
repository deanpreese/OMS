using System;
using System.Collections.Generic;

using OMS.Core.Models;
using OMS.Core.Logging;


namespace OMS.AlgoManager.Algos;

public interface IAlgo
{
    void ProcessExecution(OrderFlow order);
    void SetupAlgoTraders();        
    int AlgoGroupNumber {get;set;}
    string AlgoTraderLong{get;set;}
    string AlgoTraderShort{get;set;}
    bool UsingDebugging {get;set;}
    bool IsBackTesting {get;set;}

}
