
using System.ComponentModel.DataAnnotations;

using OMS.SharedKernel.Common;

namespace OMS.SharedKernel.DTO;


[GenerateSerializer]
[Alias("FeatureDataDTO")]
public class FeatureDataDTO
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

    [Id(8)]
    public double Open { get; set; }
    [Id(9)]
    public double High { get; set; }
    [Id(10)]
    public double Low { get; set; }
    [Id(11)]
    public double Close { get; set; }
       

}
