using OMS.Core.Interfaces;
using OMS.Core.Models;
using OMS.Infrastructure.Data.Repositories;

namespace OMS.Infrastructure;

public class TraderInfoGrain : ITraderInfoGrain
{
    private string _key_string ;
    private IUnitOfWork _unitOfWork;

    public TraderInfoGrain(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _key_string = this.GetPrimaryKeyString();
    }


    public async Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(string profile_key, int platform_id)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
        return await _unitOfWork.OrderRepository.GetLastClosedTradeByOpenPlatformID(userId, groupNum, platform_id);
    }
}
