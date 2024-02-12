
using OMS.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    OrleansStartup orleansStartup = new OrleansStartup();
    siloBuilder = orleansStartup.ConfigureSilo(siloBuilder); 
});
var startup_base = new StartupWOWeb(builder.Configuration);
startup_base.ConfigureServices(builder.Services);
var app = builder.Build();

startup_base.Configure(app, app.Environment);
app.Run();
