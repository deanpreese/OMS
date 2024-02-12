using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using OMS.Data;
using OMS.Data.Repositories;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using OMS.Core.Models;
using OMS.Services;    

namespace MigrationsManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Migrations ...");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            //optionsBuilder.EnableSensitiveDataLogging();
            //optionsBuilder.EnableDetailedErrors();


            Console.WriteLine("Complete.");

        }
    }


}