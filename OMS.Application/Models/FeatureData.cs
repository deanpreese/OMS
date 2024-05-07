
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OMS.SharedKernel.Common;

namespace OMS.Application.Models;

public class FeatureData
{
    [Key]
    public int FeatureSetID { get; set; }
    public  string FeatureSetData { get; set; }
    public  string FeatureSetName { get; set; }
    public  string FeatureNameData { get; set; }
    public long TimeTicks { get ; set; }
    public  DateTime _featureTime;
    public  string Instrument { get; set; }

    public double Open { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Close { get; set; }
       

}
