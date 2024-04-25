using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OMS.Application.Models;

public class UserProfile
{
    private DateTime _dateRegistered;

    [Key]
    public int UserID { get; set; }
    public string DisplayName { get; set; } 
    public string UserPwd { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; } 
    public string Email { get; set; } 
    public DateTime DateRegistered
    {
        get => _dateRegistered;
        set => _dateRegistered = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public int Enabled { get; set; }
    public double Leverage { get; set; }
    public int IsOpposite { get; set; }
    public int EnabledLive { get; set; }
    public int GroupRank { get; set; }
    public int GroupID { get; set; }
    public int TraderRole { get; set; }
    public ScoreCard ScoreCard { get; set; }
}