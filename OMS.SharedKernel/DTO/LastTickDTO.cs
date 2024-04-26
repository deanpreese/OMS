
namespace OMS.SharedKernel.DTO;

public class LastTickDTO
{
    private DateTime _lastTickTime;

    public int Id { get; set; }
    public DateTime LastTickTime
    {
        get => _lastTickTime;
        set => _lastTickTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
    
    public required string Symbol { get; set; }
    public double Tick { get; set; }
    public double? Bid { get; set; }
    public double? Ask { get; set; }
}
