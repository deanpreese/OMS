using System;
using System.Diagnostics;
using System;
using Microsoft.Extensions.Hosting;
using System.Drawing.Printing;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

namespace Testing;


public class DataRunner
    {
        private TestingDataService  _dataService;  

        public DataRunner() 
        {
            _dataService = new TestingDataService();
        }


        public async Task ExecuteOrdersFromCsv(string csvFilePath)
        {
            var lines = await File.ReadAllLinesAsync(csvFilePath);

            var orders = new List<NewOrderDTO>();
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');

                Random random = new Random();
                int rndm = random.Next(1000000, 5000000);

                NewOrderDTO o = new NewOrderDTO
                {
                    AuthToken = 1111111,
                    OrderType = OrderType.MARKET,
                    PlatformOrderID = rndm,
                    UserName = "ME",
                    Instrument = "DEMO",
                    OrderPX = 0,
                    OrderAction = 0,
                    Quantity = 0
                };

                o.UserID = int.Parse(values[0]);
                o.GroupID = int.Parse(values[1]);

                if (values[2] == "Buy")
                {
                    o.OrderAction = OrderAction.Buy;
                }
                else if (values[2] == "Sell")
                {
                    o.OrderAction = OrderAction.Sell;
                }

                o.Quantity = int.Parse(values[3]);
                o.OrderPX = double.Parse(values[4]);
                o.Instrument = values[5];

                orders.Add(o);
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await RunOrderListAsync(orders);
            stopwatch.Stop();
            Console.WriteLine($"Complete  {stopwatch.Elapsed} for {orders.Count} Orders  - {stopwatch.Elapsed.TotalMilliseconds/orders.Count * 1000} ");
            Console.ReadLine();

        }


        public async Task DataRunnerFromDB(int traders, int trades, int group_num)
        {
            List<NewTraderDTO> trader_list = await GetTraders(traders, group_num);
            List<NewOrderDTO> order_list = _dataService.GenerateOrders(trader_list, trades, group_num);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await RunOrderListAsync(order_list);
            stopwatch.Stop();
            Console.WriteLine($"Complete  {stopwatch.Elapsed} for {order_list.Count} Orders  - {stopwatch.Elapsed.TotalMilliseconds/order_list.Count *1000} ");
            Console.ReadLine();
        }


        public async Task DataRunnerAuto(int traders, int trades, int groups)
        {
           

            List<NewTraderDTO> trader_list = await _dataService.GenerateTraders(traders, groups);
            List<NewTraderDTO> verified_traders = await _dataService.VerifyTraders(trader_list);
            List<NewOrderDTO> order_list = _dataService.GenerateOrders(verified_traders, trades, groups);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await RunOrderListAsync(order_list);
            stopwatch.Stop();
            Console.WriteLine($"Complete  {stopwatch.Elapsed} for {order_list.Count} Orders  - {stopwatch.Elapsed.TotalMilliseconds/order_list.Count *1000} ");
            Console.ReadLine();
        }


        public async Task<List<NewTraderDTO>> GetTraders(int traders, int groupNumber)
        {

            List<NewTraderDTO> newTraders = new List<NewTraderDTO>();
            List<UserProfile> trader_list = new List<UserProfile>();

            
            trader_list = await OMSClient.GetTraders(groupNumber);
            
            if (trader_list.Count > 0)
            {
                
                foreach (var trader in trader_list)
                {
                    newTraders.Add(new NewTraderDTO
                    {
                        UserID = trader.UserID,
                        GroupID = trader.GroupID,
                        DisplayName = trader.DisplayName,
                        UserPwd = trader.UserPwd,
                        Email = trader.Email
                    });

                    Console.WriteLine($"Trader: {trader.DisplayName} - {trader.UserID} - {trader.GroupID}");
                }
            }
            return newTraders;
        }


        public async Task RunOrderListAsync(List<NewOrderDTO> orderList)
        {
            await Task.Run(async () =>
            {
                int on1 = orderList.Count ;

                foreach (var currentOrder in orderList)
                {
                    await _dataService.SendOrderAsync(currentOrder);

                    on1 -= 1;
                    Console.WriteLine($"Remain  {on1} ");
                    //Thread.Sleep(125);
                }
            });
        }




    }
    