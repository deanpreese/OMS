using System.Text;
using Newtonsoft.Json;
using OMS.Core.WebAPIClient;
using OMS.Core.Models;
using OMS.Core.Common;
using OMS.Grains.Interfaces;



namespace Testing;

public class TestingDataService
{
     IClusterClient _client;
    bool _usingGrains = false;

    public TestingDataService(IClusterClient client, bool usingGrains) 
    {
        _client = client;
        _usingGrains = usingGrains;
    }

    
    public async Task<int> SendOrderAsync(NewOrder order)
    {
        if (_usingGrains)
        {
            string o_s = order.UserID+"_"+order.GroupID;
            Console.WriteLine($"Grain: {o_s}");
            var order_grain = _client.GetGrain<IOrderGrain>(o_s);
            int rtn_code = await order_grain.ProcessOrder(order);
            return rtn_code;
        }else
        {
            int sendOrderResult = await OMSClient.SendOrderAsync(order);
            //Thread.Sleep(1000);    
            return sendOrderResult;
        }
    }



    public async Task<int> AddTraderToPlatform(NewTrader newTrader)
    {
        if (_usingGrains)
        {
            string n_t = "new_trader";
            Console.WriteLine($"AdminGrain: {n_t}");
            var admin_grain = _client.GetGrain<IAdminGrain>(n_t);
            int rtn_code = await admin_grain.AddNewTrader(newTrader);
            return rtn_code;
        }else
        {
            int trader_id = await OMSClient.AddTraderAsync(newTrader);
            return trader_id;
        }
    }


    public  async Task<int> VerifyTraderToPlatform(UserInfo trader)
    {
        if (_usingGrains)
        {
            string n_t = "new_trader";
            Console.WriteLine($"AdminGrain: {n_t}");
            var admin_grain = _client.GetGrain<IAdminGrain>(n_t);

            int rtn_code = await admin_grain.AuthenticateTrader(trader);
            return rtn_code;
        }else
        {
            int trader_id = await OMSClient.AuthenticateTraderAsync(trader);
            return trader_id;
        }
    }


    // -------------------------------------------------------------
    public  async Task<List<NewTrader>> GenerateTraders(int traders, int group)
    {
        List<NewTrader> generated_traders = new List<NewTrader>();
        for (int i = 0; i < traders; i++)
        {
            NewTrader newTrader = new NewTrader
            {
                UserID = 0,
                GroupID = group,
                UserPwd= "abc",
                DisplayName ="Gen Display",
                FirstName = "Gen First",
                LastName = "Gen Last",
                Email = "gen@example.com"
            };

            int  trader = await AddTraderToPlatform(newTrader);
            newTrader.UserID = trader;
            generated_traders.Add(newTrader);
        }
        return generated_traders;
    }


    // -------------------------------------------------------------
    public async Task<List<NewTrader>> VerifyTraders(List<NewTrader> traders)
    {
        List<NewTrader> verified_traders = new List<NewTrader>();

        foreach (var trader in traders)
        {

            UserInfo u = new UserInfo();
            u.UserID = trader.UserID;
            u.GroupID = trader.GroupID;
            u.Password = trader.UserPwd;

            
            int tid = await VerifyTraderToPlatform(u);

            if (tid == 0)
            {
                Console.WriteLine($"Auth Failed.  { trader.UserID}" );
            }else
            {    
                Console.WriteLine($"Auth Success.  { trader.UserID}" );
                verified_traders.Add(trader);
            }
        }
        return verified_traders;    
    }


    // -------------------------------------------------------------
    public  List<NewOrder> GenerateOrders(List<NewTrader> newTraders, int trades, int groups)
    {
        Random random = new Random();
        List<NewOrder> generated_orders = new List<NewOrder>();

        foreach (NewTrader trader in newTraders)
        {
            for (int groupNumber = 0; groupNumber <= groups; groupNumber++)
            {
                for (int i = 0; i < trades; i++)
                {

                    NewOrder BuyOrder = new NewOrder
                    {
                        AuthToken = 1111111,
                        OrderType = OrderType.MARKET,
                        PlatformOrderID = random.Next(1000000, 5000000),
                        UserID = trader.UserID,
                        GroupID = trader.GroupID,
                        UserName = "ME",
                        Instrument = "DEMO",
                        OrderPX = random.Next(10, 30),
                        OrderAction = OrderAction.Buy,
                        Quantity = 100
                    };

                    NewOrder SellOrder = new NewOrder
                    {
                        AuthToken = 1111111,
                        OrderType = OrderType.MARKET,
                        PlatformOrderID = random.Next(1000000, 5000000),
                        UserID = trader.UserID,
                        GroupID = trader.GroupID,
                        UserName = "ME",
                        Instrument = "DEMO",
                        OrderPX = random.Next(10, 30),
                        OrderAction = OrderAction.Sell,
                        Quantity = 100
                    };

                    generated_orders.Add(BuyOrder);
                    generated_orders.Add(SellOrder);

                }
            }
        }

        
        Random rng = new Random();
        int n = generated_orders.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            NewOrder value = generated_orders[k];
            generated_orders[k] = generated_orders[n];
            generated_orders[n] = value;
        }
        
        
        foreach (NewOrder no in generated_orders)
        {
            Console.WriteLine("Order: " + no.UserID + " " + no.OrderAction + " " + no.OrderPX + " " + no.Quantity);
        }
        return generated_orders;

    }



}
