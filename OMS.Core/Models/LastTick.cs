namespace OMS.Core.Models;


[GenerateSerializer]
[Alias("LastTick")]
public class LastTick
{
    [Id(6)]
    private DateTime _lastTickTime;

    [Id(0)]
    public int Id { get; set; }
    [Id(1)]
    public DateTime LastTickTime
    {
        get => _lastTickTime;
        set => _lastTickTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    
    [Id(2)]
    public required string Symbol { get; set; }
    [Id(3)]
    public double Tick { get; set; }
    [Id(4)]
    public double? Bid { get; set; }
    [Id(5)]
    public double? Ask { get; set; }
}
