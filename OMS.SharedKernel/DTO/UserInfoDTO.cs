namespace OMS.SharedKernel.DTO;

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

    public int UserID { get; set; }
    public string Password { get; set; }
    public int GroupID { get; set; }
   
}