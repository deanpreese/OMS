namespace OMS.Core.Common;

public enum OrderType : int
{
    MARKET = 2,
    Limit = 1 ,
    STOP = 0,
    OPEN = 100,
    CLOSE = 99 ,
    NONE = -1
}