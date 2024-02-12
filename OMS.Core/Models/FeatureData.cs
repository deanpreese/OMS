
namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("FeatureData")]
public class FeatureData
{
    [Id(3)]
    private DateTime _timeStamp;

    [Id(0)]
    public DateTime TimeStamp
    {
        get => _timeStamp;
        set => _timeStamp = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    [Id(1)]
    public int FeatureSetID { get; set; }
    [Id(2)]
    public required string FeatureSetData { get; set; }

}
