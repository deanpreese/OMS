using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OMS.Application.Models;

public class ActivityLog
{
    [Key]
    public int ActivityID { get; set; }
    public int UserID { get; set; }
    public int GroupID { get; set; }
    private DateTime _loginTime;
    public DateTime LoginTime
    {
        get { return _loginTime; }
        set { _loginTime = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
    }
    public int SessionID { get; set; }
    public string SessionGuid { get; set; } = Guid.NewGuid().ToString();
}