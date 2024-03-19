using OMS.SharedKernel.DTO;
using OMS.Core.Models;

namespace OMS.Core.Interfaces;

public interface ITraderRepository
{
    Task<int> VerifyModelTrader(NewTraderDTO user);
    Task<int> AddTraderAsync(NewTraderDTO user);
    Task<int> AuthenticateTraderAsync(int userID, string password, int groupNumber);
    Task<List<UserProfile>> GetUserProfileListAsync(int GroupNumber);
    Task<List<UserProfile>> GetUserProfileAsync(int TraderID , int GroupNumber);   
    Task<List<ActivityLog>> GetActivityLogEntriesAsync(int UserID );
    Task AddLogEntryAsync(ActivityLog log);


}
