using OMS.Core.Models;
using OMS.Data;
using OMS.Services.Trading;
using OMS.Services.Queue;
using Microsoft.Extensions.Logging;
using OMS.Core.Interfaces;


namespace OMS.Services.Trading;

public class UserService : IUserService
{
    private readonly ITradingService _trader_service;
    private readonly ILogger<UserService> _logger;
    private readonly OrderManagementDbContext _context;
    private readonly NewOrderChannelService _newOrderChannelService;
    IUnitOfWork _unitOfWork;

    public UserService(ITradingService tradingService,
        ILogger<UserService> logger, OrderManagementDbContext context, 
         NewOrderChannelService newOrderChannelService, IUnitOfWork unitOfWork )
    {

        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _newOrderChannelService = newOrderChannelService;
        _unitOfWork = unitOfWork;
        
    }

    public async Task<int> AddNewTrader(NewTrader newTrader)
    {
       int traderID = 0;

        try
        {
            _logger.LogInformation("Adding new trader...");
            // Use TraderRepository to add a new trader
            traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            // Commit transaction
            _unitOfWork.Commit();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }

        _logger.LogInformation($"Trader added  {traderID}  ");
        return traderID;
    }

    public async Task<int> AuthenticateTrader(UserInfo userInfo)
    {
         int auth_code = 0;    

        if (userInfo.Password != null)
        {
            auth_code = await _unitOfWork.TraderRepository.AuthenticateTraderAsync(userInfo.UserID, userInfo.Password, userInfo.GroupID);    
            _unitOfWork.Commit();
        }    

        return auth_code;
    }

    public async Task<int> VerifyAndAddByDisplayName(NewTrader newTrader)
    {
        int t_v = await _unitOfWork.TraderRepository.VerifyModelTrader(newTrader);

        if( t_v == 0) 
        {
            t_v = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            _unitOfWork.Commit();
        }
        return t_v;
    }
}
