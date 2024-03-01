using OMS.Core.Models;
using OMS.Services.Trading;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using OMS.Data;
using OMS.Services.Common;
using OMS.Core.Common;
using Orleans.Streams;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OMS.Services.Data;
using OMS.Services.Queue;
using OMS.Grains.Interfaces;

using Algo.Trader.Models;
using System.Text;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;
using OMS.Core.WebAPIClient;

namespace Algo.Trader.Utility;


public class AlgoConfig
{
    AlgoData _algoData ;
    string _filePath ;
    IClusterClient _clusterClient;

    public AlgoConfig(string filePath, IClusterClient clusterClient)
    {
        _algoData = new AlgoData();
        _filePath = filePath;
        _clusterClient = clusterClient;
    }

    public async Task<AlgoData> GetAlgoData()
    {
        await LoadConfig(_filePath);
        return await VerifyAlgoTraders();
    }

    private async Task<AlgoData> LoadConfig(string filePath)
    {
        string jsonString = await File.ReadAllTextAsync(filePath);
        _algoData = await System.Text.Json.JsonSerializer.DeserializeAsync<AlgoData>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine($"{_algoData.algoname} Started" + jsonString) ;   
        return  _algoData;
    }

    private async Task<AlgoData> VerifyAlgoTraders()
    {

        NewTrader n_trader = new NewTrader
        {
            UserID = 0,
            DisplayName = _algoData.algoname,
            GroupID = _algoData.group,
            UserPwd = "abc",
            FirstName = "Algo",
            LastName = _algoData.algoname,
            Email = "abc@abc"
        };

        int t_v = 0;
        await Task.Run(async () => 
        {  
            t_v = await OMSClient.VerifyModelTrader(n_trader);
            Thread.Sleep(2000);
        });

        
            
        _algoData.algo_traderId = t_v;
        
        Console.WriteLine("Trader " + t_v);

        return _algoData;
    }



}
