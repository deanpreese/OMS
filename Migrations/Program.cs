using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using OMS.Application.Interfaces;
using OMS.Application.Common;
using OMS.Application.Models;
using OMS.Infrastructure.Data;

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

            Console.WriteLine("Complete.");

        }
    }


}