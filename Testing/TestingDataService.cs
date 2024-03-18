using System.Text;
using Newtonsoft.Json;
using OMS.Core.WebAPIClient;
using OMS.Core.Models;
using OMS.Core.Common;
using OMS.Core.Interfaces;



namespace Testing;

public class TestingDataService
{

    
    public async Task<int> SendOrderAsync(NewOrderDTO order)
    {
            int sendOrderResult = await OMSClient.SendOrderAsync(order);
            //Thread.Sleep(1000);    
            return sendOrderResult;
    }



    public async Task<int> AddTraderToPlatform(NewTraderDTO newTrader)
    {
            int trader_id = await OMSClient.AddTraderAsync(newTrader);
            return trader_id;
     
    }


    public  async Task<int> VerifyTraderToPlatform(UserInfoDTO trader)
    {
            int trader_id = await OMSClient.AuthenticateTraderAsync(trader);
            return trader_id;
    }


    // -------------------------------------------------------------
    public  async Task<List<NewTraderDTO>> GenerateTraders(int traders, int group)
    {
        List<NewTraderDTO> generated_traders = new List<NewTraderDTO>();
        for (int i = 0; i < traders; i++)
        {
            NewTraderDTO newTrader = new NewTraderDTO
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
    public async Task<List<NewTraderDTO>> VerifyTraders(List<NewTraderDTO> traders)
    {
        List<NewTraderDTO> verified_traders = new List<NewTraderDTO>();

        foreach (var trader in traders)
        {

            UserInfoDTO u = new UserInfoDTO();
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
    public  List<NewOrderDTO> GenerateOrders(List<NewTraderDTO> newTraders, int trades, int groups)
    {
        Random random = new Random();
        List<NewOrderDTO> generated_orders = new List<NewOrderDTO>();

        foreach (NewTraderDTO trader in newTraders)
        {
            for (int groupNumber = 0; groupNumber <= groups; groupNumber++)
            {
                for (int i = 0; i < trades; i++)
                {

                    NewOrderDTO BuyOrder = new NewOrderDTO
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

                    NewOrderDTO SellOrder = new NewOrderDTO
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
            NewOrderDTO value = generated_orders[k];
            generated_orders[k] = generated_orders[n];
            generated_orders[n] = value;
        }
        
        
        foreach (NewOrderDTO no in generated_orders)
        {
            Console.WriteLine("Order: " + no.UserID + " " + no.OrderAction + " " + no.OrderPX + " " + no.Quantity);
        }
        return generated_orders;

    }



}
