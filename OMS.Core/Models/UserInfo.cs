
namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("UserInfo")]

public class UserInfo
{
    public UserInfo()
    {
    }

    public UserInfo(int userID, string passwd, int groupNum)
    {
        UserID = userID ;
        Password = passwd;
        GroupNumber = groupNum ;
    }

    [Id(0)]
    public int UserID { get; set; }
    [Id(1)]
    public string? Password { get; set; }
    [Id(2)]
    public int GroupNumber { get; set; }

}