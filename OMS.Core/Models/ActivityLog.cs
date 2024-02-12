using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("ActivityLog")]
public class ActivityLog
{
    [Key]
    [Id(0)]
    public int ActivityID { get; set; }
    [Id(1)]
    public int UserID { get; set; }
    [Id(2)]
    public int GroupID { get; set; }

    [Id(3)]
    private DateTime _loginTime;
    public DateTime LoginTime
    {
        get { return _loginTime; }
        set { _loginTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }

    [Id(4)]
    public int SessionID { get; set; }
    [Id(5)]
    public string SessionGuid { get; set; } = Guid.NewGuid().ToString();
}