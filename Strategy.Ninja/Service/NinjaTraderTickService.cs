using Microsoft.Extensions.Hosting;
using NinjaTrader.Client;


namespace Strategy.Trading.Service;

public class NinjaTraderTickService : BackgroundService
{

    private static double askPriceReceive, bidPriceReceive, lastPriceReceive, priceSend;
    private static bool disposed, shuttingDown, sendLive, sendingData, receivingData;
    private static string instrumentReceive, instrumentSend;
    private static int mdSubscribed, size;
    private static Client myClient;
    private static System.Threading.Timer timerReceive, timerSend;


    public NinjaTraderTickService()
    {

        
    }

    public override  Task StopAsync(CancellationToken cancellationToken)
    {

        myClient.UnsubscribeMarketData(instrumentReceive);
		myClient.TearDown();
        Console.WriteLine("Shutting Down...");

        return base.StopAsync(cancellationToken);
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

        

        int connect = myClient.Connected(1);
        Console.WriteLine(string.Format("{0} | connect: {1}", DateTime.Now, connect.ToString()));

        if (connect != 0)
        {
            Console.WriteLine("Error: Could not connect to NinjaTrader.");
            myClient.UnsubscribeMarketData(instrumentReceive);
		    myClient.TearDown();
            return Task.CompletedTask;
        }
        
        try
        {
            myClient.SubscribeMarketData(instrumentReceive);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message  + "Error: Could not subscribe to market data.");
            myClient.UnsubscribeMarketData(instrumentReceive);
		    myClient.TearDown();
            return Task.CompletedTask;
        }
        

        timerReceive =  new System.Threading.Timer(MarketDataTimerElapsed, null, 0, 2000);
        
    	

        return base.StartAsync(cancellationToken);
    }

    private void MarketDataTimerElapsed(object? state)
    {
        askPriceReceive = myClient.MarketData(instrumentReceive, 2);
        bidPriceReceive = myClient.MarketData(instrumentReceive, 1);
        lastPriceReceive = myClient.MarketData(instrumentReceive, 0);
        Console.WriteLine(string.Format("{0} | {1} | Last: {2}, Ask: {3}, Bid: {4}", DateTime.Now, instrumentReceive, lastPriceReceive, askPriceReceive, bidPriceReceive));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("NinjaTraderService is starting.");
        return Task.CompletedTask;
    }
}
