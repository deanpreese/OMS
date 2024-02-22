
using System.ComponentModel.DataAnnotations;

namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("FeatureData")]
public class FeatureData
{
    [Key]
    [Id(1)]
    public int FeatureSetID { get; set; }
    [Id(2)]
    public required string FeatureSetData { get; set; }
    [Id(3)]
    public required string FeatureSetName { get; set; }
    [Id(4)]
    public required string FeatureNameData { get; set; }
    [Id(5)]
    public long TimeTicks { get ; set; }
    [Id(6)]
    public  DateTime _featureTime;
    [Id(7)]
    public required string Instrument { get; set; }
       

}
