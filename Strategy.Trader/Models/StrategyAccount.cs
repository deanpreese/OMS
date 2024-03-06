
namespace Strategy.Trader.Models;


public class StrategyAccount
{
    public string strategy_class {get;set;}
    public string strategy_name {get;set;}
    public int group {get;set;}
    public int strategy_traderId {get;set;}
    public int orders_per_direction {get;set;}

    public string strategy_grain_key() {
        return strategy_traderId + "_" + group.ToString();
    }
}