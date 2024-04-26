
using System.ComponentModel.DataAnnotations;

using OMS.SharedKernel.Common;

namespace OMS.SharedKernel.DTO;


public class FeatureDataDTO
{
    public int FeatureSetID { get; set; }
    public required string FeatureSetData { get; set; }
    public required string FeatureSetName { get; set; }
    public required string FeatureNameData { get; set; }
    public long TimeTicks { get ; set; }
    public  DateTime _featureTime;
    public required string Instrument { get; set; }

    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
       

}
