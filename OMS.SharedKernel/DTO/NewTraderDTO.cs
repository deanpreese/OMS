

namespace OMS.SharedKernel.DTO;


[GenerateSerializer]
[Alias("NewTraderDTO")]
public class NewTraderDTO
{
    [Id(0)]
    public int GroupID {get;set;}
    [Id(1)]
    public int UserID { get; set; }
    [Id(2)]
    public string DisplayName { get; set; }
    [Id(3)]
    public string UserPwd { get; set; }
    [Id(4)]
    public string FirstName { get; set; }
    [Id(5)]
    public string LastName { get; set; }
    [Id(6)]
    public string Email { get; set; } 

}
