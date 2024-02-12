using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;

using OMS.Core.Common;
using OMS.Core.Models;
using OMS.Core.WebAPIClient;
using Orleans.Configuration;

using Spectre.Console;

namespace Testing
{
    class Program
    {
            static async Task Main(string[] args)
            {

                IClusterClient client =null;

/*
                using IHost host = Host.CreateDefaultBuilder(args)
                    .UseOrleansClient(client =>
                    {
                        client.Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "OrleansBasics";
                        });
                        client.UseAdoNetClustering(options => 
                        {
                                options.Invariant = "Npgsql";
                                options.ConnectionString = "host=10.0.0.147;database=orleans;password=abc;username=orleansuser";
                        });

                        client.AddMemoryStreams(PlatformConstants.OrderStreamProvider);
                        
                    })
                    .UseAnsiConsoleLifetime().ConfigureServices(services =>
                    {
                    
                    })
                    .Build();

                await host.StartAsync();

                var client = host.Services.GetRequiredService<IClusterClient>();
*/


                AnsiConsole.WriteLine(" ");
                AnsiConsole.MarkupLine("[green] TestRunner [/]");
                AnsiConsole.WriteLine(" ");
                AnsiConsole.WriteLine("0 - AutoGenData");
                AnsiConsole.WriteLine("1 - Run Standard Order Tests -- webAPI only");
                AnsiConsole.WriteLine("2 - Read CSV");
                AnsiConsole.WriteLine("3 - Pull Traders from DB");

                int opt = 0;
                bool useGrain = false;

                try
                {
                    string? input = Console.ReadLine();
                    opt = input != null ? int.Parse(input) : 0;
                    AnsiConsole.WriteLine(" ");
                    AnsiConsole.WriteLine("1   Use Grains");
                    AnsiConsole.WriteLine("2   WebAPI");
                    string? grains_input = Console.ReadLine();
                    AnsiConsole.WriteLine(" ");

                    int g_yes = grains_input != null ? int.Parse(grains_input) : 0; 

                    if (g_yes == 1)
                    {
                        useGrain = true;
                    }

                }
                catch (Exception)
                {
                    AnsiConsole.WriteLine("Invalid Option");
                    return;
                }

                switch (opt)
                {
                    case 0:
                        AnsiConsole.WriteLine("Provide 3 numbers CSV- (Traders, Trades, Groups)");
                        string? input = Console.ReadLine();

                        if (input != null)
                        {
                            int[] nums = Array.ConvertAll(input.Split(','), int.Parse);

                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner(client, useGrain);
                                await dataRunner.DataRunnerAuto(nums[0], nums[1], nums[2]);
                            });
                        }
                        break;

                    case 1:
                        await Task.Run(async () => {
                            StandardOrderTests orderProcessor = new StandardOrderTests(client, useGrain);
                            await orderProcessor.RunAll(200);
                        });

                        break;


                    case 2:
                        AnsiConsole.WriteLine("Provide CSV file");
                        string? filename = Console.ReadLine();

                        if (filename != null)
                        {
                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner(client, useGrain);
                                await dataRunner.ExecuteOrdersFromCsv(filename);
                            });
                        }

                        break;

                    case 3:
                        AnsiConsole.WriteLine("Provide 2 numbers CSV - Traders , Trades, Group_Num ");
                        string? input_db = Console.ReadLine();

                        if (input_db != null)
                        {
                            int[] nums = Array.ConvertAll(input_db.Split(','), int.Parse);                                

                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner(client, useGrain);
                                await dataRunner.DataRunnerFromDB(nums[0], nums[1], nums[2]);
                            });
                        }

                        break;


                    default:
                        break;
                }


                AnsiConsole.MarkupLine("[Green]Process Complete - Press any key to exit[/]");
                Console.ReadLine();

            }
    }

    
    
}