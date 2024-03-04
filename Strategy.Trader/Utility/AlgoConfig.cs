using OMS.Core.Models;

using System.Text;
using OMS.Core.WebAPIClient;
using Strategy.Trader.Models;

namespace Strategy.Trader.Utility;


public class AlgoConfig
{
    AlgoData _algoData;
    string _filePath;
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

        Console.WriteLine($"{_algoData.algoname} Started" + jsonString);
        return _algoData;
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
