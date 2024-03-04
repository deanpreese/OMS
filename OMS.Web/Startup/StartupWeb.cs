using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Text.Json.Serialization;
using OMS.Data;
using Microsoft.EntityFrameworkCore;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Services.Trading;
using OMS.Services.Data;
using OMS.Services.Queue;
using OMS.Data.Repositories;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace OMS.Web.Startup;

public class StartupWeb
{
    public IConfiguration Configuration { get; }
    
    public StartupWeb(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IServiceCollection ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddMvc();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpsRedirection(opt => opt.HttpsPort = 44300);
        return services;
    }

    public IApplicationBuilder Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
        app.UseRouting();
        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseStaticFiles();
        //app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        return app;


    }
}