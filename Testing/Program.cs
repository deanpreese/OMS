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
using Orleans.Hosting;

namespace Testing
{
    class Program
    {
            static async Task Main(string[] args)
            {
                Console.WriteLine(" ");
                Console.WriteLine("TestRunner");
                Console.WriteLine(" ");
                Console.WriteLine("0 - AutoGenData -- Traders, Groups and Trades");
                Console.WriteLine("1 - Run Standard Order Tests");
                Console.WriteLine("2 - Read from CSV");
                Console.WriteLine("3 - Pull Traders from DB");
                int opt = 0;

                try
                {
                    string input = Console.ReadLine();
                    opt = input != null ? int.Parse(input) : 0;


                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Option");
                    return;
                }

                // Process Options    

                switch (opt)
                {
                    case 0:
                        Console.WriteLine("Provide 3 numbers CSV- (Traders, Trades, Groups)");
                        string input = Console.ReadLine();

                        if (input != null)
                        {
                            int[] nums = Array.ConvertAll(input.Split(','), int.Parse);

                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner();
                                await dataRunner.DataRunnerAuto(nums[0], nums[1], nums[2]);
                            });
                        }
                        break;

                    case 1:
                        await Task.Run(async () => {
                            StandardOrderTests stdOrderTests = new StandardOrderTests();
                            await stdOrderTests.RunAll(200);
                        });

                        break;


                    case 2:
                        Console.WriteLine("Provide CSV file");
                        string filename = Console.ReadLine();

                        if (filename != null)
                        {
                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner();
                                await dataRunner.ExecuteOrdersFromCsv(filename);
                            });
                        }
                        break;

                    case 3:
                        Console.WriteLine("Provide 2 numbers CSV - Traders , Trades, Group_Num ");
                        string input_db = Console.ReadLine();

                        if (input_db != null)
                        {
                            int[] nums = Array.ConvertAll(input_db.Split(','), int.Parse);                                

                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner();
                                await dataRunner.DataRunnerFromDB(nums[0], nums[1], nums[2]);
                            });
                        }

                        break;


                    default:
                        break;
                }


                Console.WriteLine("Process Complete - Press any key to exit");
                Console.ReadLine();

            }
    }

    
    
}