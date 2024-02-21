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

using Algo.Algorithms.Models;
using Algo.Algorithms.Services;
using System.Text;
using System.Text.Json;
using System.IO;

namespace Algo.Algorithms;


public class AlgoConfig
{
    AlgoData _algoData ;
    string _filePath ;
    private readonly IGrainFactory _grainFactory;

    IClusterClient _clusterClient;

    public AlgoConfig(string filePath, IGrainFactory grainFactory, IClusterClient clusterClient)
    {
        _algoData = new AlgoData();
        _grainFactory = grainFactory;
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
        _algoData = await JsonSerializer.DeserializeAsync<AlgoData>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine($"{_algoData.algoname} Started" + jsonString) ;   
        return  _algoData;
    }

    private async Task<AlgoData> VerifyAlgoTraders()
    {

        NewTrader n_trader = new NewTrader
        {
            UserId = 0,
            DisplayName = _algoData.algoname,
            Group = _algoData.group,
            UserPwd = "abc",
            FirstName = "Algo",
            LastName = _algoData.algoname,
            Email = "abc@abc"
        };

        int t_v = 0;
        await Task.Run(async () => 
        {   
            IAdminGrain admin =  _clusterClient.GetGrain<IAdminGrain>(_algoData.algoname);
            t_v = await admin.AuthByDisplayName(n_trader);

            if (t_v == 0)
            {
                IAdminGrain admin2 =  _clusterClient.GetGrain<IAdminGrain>(_algoData.algoname+"22");
                t_v = await admin2.AddNewTrader(n_trader);
            }

            Thread.Sleep(2000);
        });

        
            
        _algoData.algo_traderId = t_v;
        
        Console.WriteLine("Trader " + t_v);

        return _algoData;
    }
}
