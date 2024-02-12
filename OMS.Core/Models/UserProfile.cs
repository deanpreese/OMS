using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("UserProfile")]
public class UserProfile
{
    [Id(14)]
    private DateTime _dateRegistered;

    [Key]
    [Id(0)]
    public int UserID { get; set; }
    [Id(1)]
    public string? DisplayName { get; set; } 
    [Id(2)]
    public string? UserPwd { get; set; }
    [Id(3)]
    public string? FirstName { get; set; } 
    [Id(4)]
    public string? LastName { get; set; } 
    [Id(5)]
    public string? Email { get; set; } 
    [Id(6)]
    public DateTime DateRegistered
    {
        get => _dateRegistered;
        set => _dateRegistered = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    [Id(7)]
    public int Enabled { get; set; }
    [Id(8)]
    public double Leverage { get; set; }
    [Id(9)]
    public int IsOpposite { get; set; }
    [Id(10)]
    public int EnabledLive { get; set; }
    [Id(11)]
    public int GroupRank { get; set; }
    [Id(12)]
    public int TraderGroup { get; set; }
    [Id(13)]
    public int TraderRole { get; set; }
}