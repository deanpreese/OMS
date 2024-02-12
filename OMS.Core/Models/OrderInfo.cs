namespace OMS.Core.Models;

[GenerateSerializer]
[Alias("OrderInfo")]

public class OrderInfo
{
    public OrderInfo()
    {
    }

    public OrderInfo(int userID, string passwd, int groupNum, OrderInfoType intoType)
    {
        UserID = userID ;
        Password = passwd;
        GroupNumber = groupNum ;
        InfoType = intoType;
    }

    [Id(0)]
    public int UserID { get; set; }
    [Id(1)]
    public string? Password { get; set; }
    [Id(2)]
    public int GroupNumber { get; set; }
    [Id(3)]
    public OrderInfoType InfoType { get; set; }

}



public enum OrderInfoType : int
{
    OPEN = 1,
    CLOSED = 2 ,
}