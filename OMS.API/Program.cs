
using OMS.API.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseOrleans((ctx, siloBuilder) =>
{
    OrleansStartup orleansStartup = new OrleansStartup();
    siloBuilder = orleansStartup.ConfigureSilo(siloBuilder); 
});


var startup_web = new StartupWeb(builder.Configuration);
startup_web.ConfigureServices(builder.Services);

var startup_base = new StartupServices(builder.Configuration);
startup_base.ConfigureServices(builder.Services);


var app = builder.Build();
startup_web.Configure(app, app.Environment);
startup_base.Configure(app, app.Environment);

await app.RunAsync();
//app.Run();
