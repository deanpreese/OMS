using OMS.DataManager;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GenericMessageBus<FeatureDataDTO>>();
builder.Services.AddHostedService<FeatureDataProcessor>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost(PlatformConstants.PROCESS_RAW_FEATURE_DATA, async (GenericMessageBus<FeatureDataDTO> featureDataBus, FeatureDataDTO featureData ) =>
      {
        await featureDataBus.PublishAsync(featureData);
        return Results.Ok();
      })
      .WithName("ProcessRawFeatureData")
      .WithOpenApi();

app.Run();
