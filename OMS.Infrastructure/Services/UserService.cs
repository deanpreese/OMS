using OMS.Application.Models;
using Microsoft.Extensions.Logging;
using OMS.Application.Interfaces;
using OMS.Infrastructure.Data;
using OMS.Infrastructure.Queue;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;
using OMS.Infrastructure.Interfaces;

namespace OMS.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly ITradingService _trader_service;
    private readonly ILogger<UserService> _logger;
    private readonly OrderManagementDbContext _context;
    private readonly NewOrderChannelService _newOrderChannelService;
    IUnitOfWork _unitOfWork;

    public UserService(ITradingService tradingService,
        ILogger<UserService> logger, OrderManagementDbContext context,
         NewOrderChannelService newOrderChannelService, IUnitOfWork unitOfWork)
    {

        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _newOrderChannelService = newOrderChannelService;
        _unitOfWork = unitOfWork;

    }

    public async Task<int> AddNewTrader(NewTraderDTO newTrader)
    {
        int traderID = 0;
        try
        {
            _logger.LogInformation("Adding new trader...");
            traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            _unitOfWork.Commit();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
        _logger.LogInformation($"Trader added  {traderID}  ");
        return traderID;
    }

    public async Task<int> AddNewTraderScoreCard(int traderID, int groupID)
    {
         traderID = await _unitOfWork.AnalyticsRepository.AddNewTraderScorecard(traderID, groupID);
         _unitOfWork.Commit();
        _logger.LogInformation($"Trader Scorecard added  {traderID} {groupID} ");

        return traderID;

    }

    public async Task<int> AuthenticateTrader(UserInfoDTO userInfo)
    {
        int auth_code = 0;

        if (userInfo.Password != null)
        {
            auth_code = await _unitOfWork.TraderRepository.AuthenticateTraderAsync(userInfo.UserID, userInfo.Password, userInfo.GroupID);
            _unitOfWork.Commit();
        }

        return auth_code;
    }

    public async Task<int> VerifyAndAddByDisplayName(NewTraderDTO newTrader)
    {
        int t_v = await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);

        if (t_v == 0)
        {
            t_v = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            _unitOfWork.Commit();
        }
        return t_v;
    }
}
