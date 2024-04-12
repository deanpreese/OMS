
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Strategy.Ninja.Service;



var builder = WebApplication.CreateBuilder(args);

/*
builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    OrleansStartupAlgo orleansStartup = new OrleansStartupAlgo();
    siloBuilder = orleansStartup.ConfigureSilo(siloBuilder); 
});
*/
/*
 builder.Services.AddDbContext<OrderManagementDbContext>(options =>
        {
            //string conn =  "Host=127.0.0.1;Database=orders;Username=trading;Password=abc";
            string conn =  "Server=127.0.0.1;Database=orders;Username=trading;Password=abc";
            options.UseNpgsql(conn);

        },ServiceLifetime.Scoped);

*/

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 
builder.Services.AddSingleton<FeatureDataDataQueue>();
builder.Services.AddHostedService<FeatureDataProcessor>();



builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed(origin => true) // allow any origin
        .AllowCredentials()); // allow credentials

// app.UseHttpsRedirection();

app.UseRouting();
//app.UseAuthorization();
//app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();