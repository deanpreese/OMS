
namespace OMS.Core.Common;

public class TrackedInstruments
{
    List<InstrumentData> instruments = new List<InstrumentData>();

    public TrackedInstruments()
    {
        instruments.Add(new InstrumentData("NZDUSD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("EURJPY", 1000, 0.01, 10000));
        instruments.Add(new InstrumentData("EURAUD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("AUDCAD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("GBPUSD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("USDCHF", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("AUDNZD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("USDJPY", 1000, 0.01, 10000));
        instruments.Add(new InstrumentData("CADCHF", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("AUDUSD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("AUDCHF", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("GBPAUD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("GBPCAD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("EURCAD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("CADJPY", 1000, 0.01, 10000));
        instruments.Add(new InstrumentData("GBPJPY", 1000, 0.01, 10000));
        instruments.Add(new InstrumentData("NZDJPY", 1000, 0.01, 10000));
        instruments.Add(new InstrumentData("EURGBP", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("EURUSD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("NZDCAD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("AUDJPY", 1000, 0.01, 10000));
        instruments.Add(new InstrumentData("CHFJPY", 1000, 0.01, 10000));
        instruments.Add(new InstrumentData("EURNZD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("GBPCHF", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("USDCAD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("GBPNZD", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("NZDCHF", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("EURCHF", 100000, 0.0001, 10000));
        instruments.Add(new InstrumentData("ES", 50, 0.25, 1));
        instruments.Add(new InstrumentData("SPY", 1, 0.1, 100));
        instruments.Add(new InstrumentData("DEMO", 1, 0.1, 100));

    }


    public InstrumentData FindData(string symbol)
    {
        InstrumentData _instrument = new InstrumentData();
        _instrument.Symbol = "NONE";
        _instrument.MinLotSize = 0;
        _instrument.PointValue = 0;

        // look up the symbol in the symbol list or database
        try
        {
            _instrument = instruments.Single(s => s.Symbol == symbol);
        }
        catch (Exception)
        {
            
        }

        return _instrument;
    }
    //-----------------------------------------------
}
