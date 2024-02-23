using System.Security.Permissions;
namespace Algo.Trader.Models;


public class AlgoData
{
    public string? algoclass {get;set;}
    public string? algoname {get;set;}
    public int group {get;set;}
    public bool usedebugging {get;set;}
    public bool backtesting {get;set;}

    public int algo_traderId {get;set;}

    public string? algo_grain() {
        return algo_traderId + "_" + group.ToString();
    }
}