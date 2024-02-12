

namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("NewTrader")]
public class NewTrader
{
    [Id(0)]
    public int Group {get;set;}
    [Id(1)]
    public int UserId { get; set; }
    [Id(2)]
    public string? DisplayName { get; set; }
    [Id(3)]
    public string? UserPwd { get; set; }
    [Id(4)]
    public string? FirstName { get; set; }
    [Id(5)]
    public string? LastName { get; set; }
    [Id(6)]
    public string? Email { get; set; } 

}
