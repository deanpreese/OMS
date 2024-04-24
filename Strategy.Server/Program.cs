using System;
using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

using Strategy.Server.Services;

using OMS.SharedKernel.Common;
using Strategy.Server;
using Strategy.Server.StrategyServices;
using Microsoft.AspNetCore.Builder;
using OMS.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ModelOrderMessageBus>();                
builder.Services.AddHostedService<WatcherService>();
builder.Services.AddHostedService<StrategyService>();  // OC


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
//app.UseOpenApi();
app.MapStrategyServerEndpoints();
app.Run();


Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();

