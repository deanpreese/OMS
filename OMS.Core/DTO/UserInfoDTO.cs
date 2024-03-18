
using OMS.Core.Common;

namespace OMS.Core.DTO;

[GenerateSerializer]
[Alias("UserInfoDTO")]

public class UserInfoDTO
{
    public UserInfoDTO()
    {
    }

    public UserInfoDTO(int userID, string passwd, int groupNum)
    {
        UserID = userID ;
        Password = passwd;
        GroupID = groupNum ;
    }

    [Id(0)]
    public int UserID { get; set; }
    [Id(1)]
    public string Password { get; set; }
    [Id(2)]
    public int GroupID { get; set; }
   
}