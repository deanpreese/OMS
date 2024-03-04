using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 


namespace Algo.Trader
{
    public interface IAlgoEngine
    {
        string EngineName();
        string AlgoDescription();

        //bool Initialize(FIXDataAdapterMT FIXConnection, string AlgoTrader, int DebugLevel, string LeverageAcount);
        bool ProcessTradeMessage(string message);
        bool ProcessGlobalLeverageOverrideMessage(int OverrideAmount, string algo);
        bool ResetOverrideLeverage(string algo);
        string ShowExclusionsList(string algo);
        void ClearExclusionsList(string algo);
        bool AddInstrumentToExclusionsList(string instrument, string algo);
        bool RemoveInstrumentFromExclusionsList(string instrument, string algo);
        bool ResetLeveragePositionsByInstrument(string instrument, string algoTrader);
        void OpenLeveragePosition(int baseLeverage, string trader, string algoTrader, int orignalOrderDirection);
        //int ProcessLeverageOrder(OrderData thisOrder);
        string LogLeverageOrderInformation(int ExeID, string trader, string algoTrader, string leverageOrder);

        //event EventHandler<PublishEvent> LogLeverageOrderCB;
    }
}
