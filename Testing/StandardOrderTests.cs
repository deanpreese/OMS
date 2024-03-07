using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

using System.Text.Json;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Net.Http.Json;

using OMS.Core.Common;
using OMS.Core;
using OMS.Core.WebAPIClient;
using OMS.Core.Models;

using System.Diagnostics;

namespace Testing
{
    public class StandardOrderTests
    {
        private TestingDataService _dataService;    
        
        public StandardOrderTests()
        {
            _dataService = new TestingDataService();
        }

        UserInfo uInfo = new(999999,"abc", 0 );

        // -------------------------------------------------------------
        private NewOrder GenerateBaseOrder()
        {
            Random random = new Random();
            int rndm = random.Next(1000000, 5000000);

            NewOrder order = new NewOrder
            {
                AuthToken = 1111111,
                OrderType = OrderType.MARKET,
                PlatformOrderID = rndm,
                UserID = uInfo.UserID,
                GroupID = uInfo.GroupID,
                UserName = "ME",
                Instrument = "DEMO",
                OrderPX = 0,
                OrderAction = 0,
                Quantity = 0
            };
            return order;
        }
       

        public async Task RunAll(int times)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < times; i++)        
            {
                await RunAll();
            }
            stopwatch.Stop();
            Console.WriteLine($"Complete  {stopwatch.Elapsed} ");
            
        }

        // -------------------------------------------------------------
        public async Task RunAll()
        {
            LongClose();
            await Task.CompletedTask;
            await Task.CompletedTask;
            LongCloseLoss();
            await Task.CompletedTask;
            ShortClose();
            await Task.CompletedTask;
            ShortCloseLoss();
            await Task.CompletedTask;
            LongAddPartialClose();
            await Task.CompletedTask;
            ShortAddPartialClose();
            await Task.CompletedTask;
            LongReverseClose();
            await Task.CompletedTask;
            ShortReverseClose();
            await Task.CompletedTask;
            
        }


        public async void LongClose()
        {
            await BuyOrder(100, 10);
            await Task.CompletedTask;
            await SellOrder(100, 11); ;
            await Task.CompletedTask;
        }

        public async void LongCloseLoss()
        {
            await BuyOrder(100, 10);
            await SellOrder(100, 9);;
        }

        public async void ShortClose()
        {
            await SellOrder(100, 10);
            await BuyOrder(100, 9);
        }

        public async void ShortCloseLoss()
        {
            await SellOrder(100, 10);
            await BuyOrder(100, 11);
        }

        public async void LongReverseClose()
        {
            await BuyOrder(100, 10);
            await SellOrder(100, 11);
            await BuyOrder(100, 10);
            await SellOrder(100, 9);
        }

        public async void ShortReverseClose()
        {
            await SellOrder(100, 10);
            await BuyOrder(100, 11);
            await SellOrder(100, 10);
            await BuyOrder(100, 9);
        }

        public async void ShortAddPartialClose()
        {
            await SellOrder(100, 10);
            await SellOrder(100, 11);
            await BuyOrder(100, 10);
            await BuyOrder(100, 9);
        }

        public async void LongAddPartialClose()
        {
            await BuyOrder(100, 10);
            await BuyOrder(100, 11);
            await SellOrder(100, 10);
            await SellOrder(100, 9);
        }


        // ========================================================================
        public async Task BuyOrder(int qty, double px)
        {
            //Thread.Sleep(500);
            NewOrder o = GenerateBaseOrder();
            o.Quantity = qty;
            o.OrderAction = OrderAction.Buy;
            o.OrderPX = px;
            int r = await _dataService.SendOrderAsync(o);    
            Console.WriteLine("BuyOrder: " + r);
        }


        public async Task SellOrder(int qty, double px)
        {
            //Thread.Sleep(500);
            NewOrder so = GenerateBaseOrder();
            so.OrderAction = OrderAction.Sell;
            so.Quantity = qty;
            so.OrderPX = px;
            int r = await _dataService.SendOrderAsync(so);
            Console.WriteLine("SellOrder: " + r);
        }





    }
}
