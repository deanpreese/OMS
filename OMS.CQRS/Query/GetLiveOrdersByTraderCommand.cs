namespace OMS.CQRS;

public class GetLiveOrdersByTraderCommand
{
    public int TraderID { get; set; }
    public int TraderGroup { get; set; }
}
