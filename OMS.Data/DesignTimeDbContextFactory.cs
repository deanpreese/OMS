using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OMS.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderManagementDbContext>
    {
        private  IConfiguration _configuration;

        public DesignTimeDbContextFactory()
        {
        }

        public DesignTimeDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public OrderManagementDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new OrderManagementDbContext(optionsBuilder.Options, _configuration);
        }

    }
}