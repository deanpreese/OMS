using Microsoft.Extensions.Hosting;
using NinjaTrader.Client;


namespace OMS.DataServer;

public class NinjaTraderService : BackgroundService
{

    private static double askPriceReceive, bidPriceReceive, lastPriceReceive, priceSend;
    private static bool disposed, shuttingDown, sendLive, sendingData, receivingData;
    private static string instrumentReceive, instrumentSend;
    private static int mdSubscribed, size;
    private static Client myClient;
    private static System.Threading.Timer timerReceive, timerSend;


    public NinjaTraderService()
    {
    }


    public override  Task StartAsync(CancellationToken cancellationToken)
    {

        myClient = new Client();
        shuttingDown = false;
        instrumentReceive = "ES 06-24";
        sendLive = true;
        sendingData = false;
        receivingData = false;
        mdSubscribed = 0;
        size = 1; 

        timerReceive =  new System.Threading.Timer(MarketDataTimerElapsed, null, 0, 2000);

        int connect = myClient.Connected(1);
        myClient.SubscribeMarketData(instrumentReceive);

        Console.WriteLine(string.Format("{0} | connect: {1}", DateTime.Now, connect.ToString()));

        Console.WriteLine("Press any key to continue...");
        Console.ReadLine();

    	myClient.UnsubscribeMarketData(instrumentReceive);
		myClient.TearDown();
        Console.WriteLine("Shutting Down...");

        return base.StartAsync(cancellationToken);
    }

    private void MarketDataTimerElapsed(object? state)
    {
        
        askPriceReceive		= myClient.MarketData(instrumentReceive, 2);
        bidPriceReceive		= myClient.MarketData(instrumentReceive, 1);
        lastPriceReceive	= myClient.MarketData(instrumentReceive, 0);
        
        Console.WriteLine(string.Format( "{0} | {1} | Last: {2}, Ask: {3}, Bid: {4}", DateTime.Now, instrumentReceive, lastPriceReceive, askPriceReceive, bidPriceReceive ));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
