
namespace Strategy.Trader.Models;


public class StrategyAccount
{
    public string strategy_class {get;set;}
    public string startegy_name {get;set;}
    public int group {get;set;}
    public bool usedebugging {get;set;}
    public bool backtesting {get;set;}

    public int strategy_traderId {get;set;}

    public string strategy_grain() {
        return strategy_traderId + "_" + group.ToString();
    }
}