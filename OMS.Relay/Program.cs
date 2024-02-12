using System.Text.Json.Serialization;
using OMS.Services.Queue;
using OMS.Services.Common;
using OMS.Relay.SignalHub;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); 

//builder.Services.AddMvc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IPlatformOrderIDGen, PlatformOrderIDGen>();    
builder.Services.AddSingleton<NewOrderQueue>();
builder.Services.AddHostedService<NewOrderProcessor>();

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
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<SignalHub>("/hubs/signal-hub");

app.Run();

