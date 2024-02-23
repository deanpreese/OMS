using OMS.Core.Models;

namespace OMS.CQRS;

public class AuthenticateTraderCommand
{
    public UserInfo userInfo { get; set; }
}
