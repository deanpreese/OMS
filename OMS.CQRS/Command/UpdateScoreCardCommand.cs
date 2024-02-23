using OMS.Core.Models;

namespace OMS.CQRS;

public class UpdateScoreCardCommand
{
    public UserInfo userInfo { get; set; }
}
