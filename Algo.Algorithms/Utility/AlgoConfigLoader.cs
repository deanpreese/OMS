using Algo.Algorithms.Models;
using System.IO;
using System.Text;
using System.Text.Json; 

namespace Algo.Algorithms;


public class AlgoConfigLoader
{
    public static async Task<AlgoData> LoadConfig(string filePath)
    {
        AlgoData _algoData = new AlgoData(); 
        string jsonString = await File.ReadAllTextAsync(filePath);
        _algoData = await JsonSerializer.DeserializeAsync<AlgoData>(new MemoryStream(Encoding.UTF8.GetBytes(jsonString))) ??
            throw new ArgumentNullException($"File {filePath} is empty.");

        Console.WriteLine($"{_algoData.algoname} Started" + jsonString) ;   
        return _algoData;
    }

}
