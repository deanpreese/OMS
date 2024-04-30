using OMS.Application.Models;
using OMS.Infrastructure.Interfaces;
using OMS.Infrastructure.Services;
using OMS.SharedKernel.DTO;

namespace Strategy.Data;

public class StrategyDataService
{
    private readonly IUserService _userService;
    private readonly IDataService _dataService;
    private readonly IAnalyticsService _analyticsService;

    public StrategyDataService(IUserService userService, IDataService dataService, 
        IAnalyticsService analyticsService)
    {
        _userService = userService;
        _dataService = dataService;
        _analyticsService = analyticsService;
    }

    public async Task<int> VerifyAndAddByDisplayName(NewTraderDTO newTrader)
    {
        int traderID = await _userService.VerifyAndAddByDisplayName(newTrader);
        return traderID;
    }

    public async Task<int> AddTraderAsync(NewTraderDTO newTrader)
    {
        int traderID = await _userService.AddNewTrader(newTrader);
        return traderID;
    }


    public async Task<List<LiveOrderDTO>> GetLiveOrdersByTrader(int traderID, int groupID)
    {
            List<LiveOrderDTO> orders = await _dataService.GetLiveOrdersByTrader(traderID, groupID);
            return orders;            
    }
    public async Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformID(int traderID, int groupID, int platformID)
    {
            ClosedTradeDTO trade = await _dataService.GetLastClosedTradeByOpenPlatformID(traderID, groupID, platformID);
            return trade;    
    }

    public async Task<ScoreCardDTO> GetTraderScoreCard(int traderID, int groupID)
    {
            ScoreCardDTO sc_dto = await _analyticsService.GetTraderScoreCardDTO(traderID, groupID);
            return sc_dto;

    }

}
