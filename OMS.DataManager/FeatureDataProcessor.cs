using OMS.Application.Models;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Services;
using OMS.Infrastructure.Interfaces;
using OMS.Application;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Channels;
using QuestDB;

namespace OMS.DataManager;

public class FeatureDataProcessor : BackgroundService
{
    private readonly GenericMessageBus<FeatureDataDTO> _featureDataBus;
    private readonly ILogger<FeatureDataProcessor> _logger;
    private readonly  ChannelReader<FeatureDataDTO> _reader;

    public FeatureDataProcessor(ILogger<FeatureDataProcessor> logger,
        GenericMessageBus<FeatureDataDTO> featureDataBus)  
    {
        _logger = logger;
        _featureDataBus = featureDataBus;
        _reader = _featureDataBus.Subscribe();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var feature_data in _reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessData(feature_data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task ProcessData(FeatureDataDTO featureDataDTO)
    {

        //SDLR310,SDBB91,SDKC91,SDKC9,ROC,ATR34,ATR32,ATR31,ATR3,
        //ATR21,ATR2,RSI,STOK1,output,outputC,actual

        //0.000441, 0.005306, 0.00513, 0.004428, -0.605464, 11.278864,
        //8.438473, .100778, 6.430623, 2.939586, 3.344793, 27.848114,
        //8.336952, -2.5, 0.0, 5048
        DateTime feature_time = new DateTime(featureDataDTO.TimeTicks);

        Console.WriteLine($"{feature_time} {featureDataDTO.FeatureSetData}");

        string[] fdn_str = featureDataDTO.FeatureNameData.Split(',');
        string[] fdd_str = featureDataDTO.FeatureSetData.Split(',');
        
        double[] fdd_d = new double[fdd_str.Length];
        foreach (var item in fdd_str)
        {
            fdd_d[Array.IndexOf(fdd_str, item)] = double.Parse(item);
        }

        using var sender = Sender.New(PlatformConstants.QUEST_DB_CONN);
        
        await sender.Table("feature_data")
            .Column("Open", featureDataDTO.Open)
            .Column("High", featureDataDTO.High)
            .Column("Low", featureDataDTO.Low)
            .Column("Close", featureDataDTO.Close)
            .Column(fdn_str[0], fdd_d[0])
            .Column(fdn_str[1], fdd_d[1])
            .Column(fdn_str[2], fdd_d[2])
            .Column(fdn_str[3], fdd_d[3])
            .Column(fdn_str[4], fdd_d[4])
            .Column(fdn_str[5], fdd_d[5])
            .Column(fdn_str[6], fdd_d[6])
            .Column(fdn_str[7], fdd_d[7])
            .Column(fdn_str[8], fdd_d[8])
            .Column(fdn_str[9], fdd_d[9])
            .Column(fdn_str[10], fdd_d[10])
            .Column(fdn_str[11], fdd_d[11])
            .Column(fdn_str[12], fdd_d[12])
            .Column(fdn_str[13], fdd_d[13])
            .Column(fdn_str[14], fdd_d[14])
            .Column(fdn_str[15], fdd_d[15])
            .AtAsync(feature_time);
                                        
        //await sender.SendAsync();

                    
                            
    }


}
