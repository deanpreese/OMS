using OMS.Core.Common;
using OMS.Core.Interfaces;
using OMS.Core.Models;

using OMS.Infrastructure.Data;

using OMS.SharedKernel.DTO;
//using OMS.SharedKernel.Grains;

namespace OMS.Infrastructure;

/*

public class TraderInfoGrain : Grain, ITraderInfoGrain
{
    private string _key_string ;
    private IUnitOfWork _unitOfWork;

    public TraderInfoGrain(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _key_string = this.GetPrimaryKeyString();
    }


    public async Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformID(string profile_key, int platform_id)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);

        ClosedTrade closedTrade = await _unitOfWork.ClosedOrderRepository.GetLastClosedTradeByOpenPlatformID(userId, groupNum, platform_id);
        ClosedTradeDTO closedTradeDTO = null;

        if (closedTrade != null)    
        {
            try 
            {
                closedTradeDTO = await DTOMapping.MapClosedOrderToClosedOrderDTO(closedTrade);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);   
            }
        }

        return closedTradeDTO;
    }

    public async Task<ScoreCardDTO> GetScoreCardAsync(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
        
        var sc_from_db = await _unitOfWork.AnalyticsRepository.GetTraderScoreCard(userId, groupNum);

        ScoreCardDTO sc = new ScoreCardDTO();
        if (sc_from_db != null)
        {
            sc = await DTOMapping.MapScorecardToScorecardDTO(sc_from_db);
        }
       return sc;
    }
}

*/