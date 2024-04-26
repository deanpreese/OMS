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

using OMS.SharedKernel.Common;
using Microsoft.AspNetCore.Builder;
using Strategy.Runner.Services;
using Strategy.Runner.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<StrategyRunnerService>();               
builder.Services.AddHostedService<StrategyService>();  
   
var app = builder.Build();
app.MapStrategyRunnerEndpoints();
app.Run();

Console.WriteLine("Press Enter to terminate...");
Console.ReadLine();

