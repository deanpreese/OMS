using OMS.Core.Interfaces;
using OMS.Core.Models;
using OMS.Core.Common;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;


namespace OMS.Infrastructure.Data.Repositories;

public class TraderRepository : ITraderRepository
{
    private readonly OrderManagementDbContext _context;

    public TraderRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddLogEntryAsync(ActivityLog log)
    {
        await _context.ActivityLogs.AddAsync(log);
        await Task.CompletedTask;
    }

    public async Task<int> AddTraderAsync(NewTraderDTO addedTrader)
    {
        UserProfile user = new UserProfile
        {
            UserID = await GetNewTraderID(addedTrader.GroupID),
            DisplayName = addedTrader.DisplayName,
            UserPwd = addedTrader.UserPwd,
            Email = addedTrader.Email,
            FirstName = addedTrader.FirstName,
            LastName = addedTrader.LastName,
            DateRegistered = DateTime.Now,
            Enabled = 1,
            EnabledLive = 0,
            Leverage = 1,
            IsOpposite = 0,
            GroupRank = 0,
            GroupID = addedTrader.GroupID,
            TraderRole = 1
        };

        ScoreCard scd = new ScoreCard
        {
            UserID = user.UserID,
            TradeXML = " ",
            GroupID = addedTrader.GroupID
        };
        await _context.UserProfiles.AddAsync(user);
        return user.UserID;

    }


    public async Task<int> AuthenticateTraderAsync(int userID, string password, int groupNumber)
    {
        Guid g = Guid.NewGuid();
        byte[] gb = g.ToByteArray();
        int auth_token = Math.Abs(BitConverter.ToInt32(gb, 0));

        List<UserProfile> userList = await GetUserProfileAsync(userID ,groupNumber);

        if (userList.Count > 0)
        {
            UserProfile u = userList.FirstOrDefault();
            if (u != null && u.UserPwd != null && u.UserPwd != password)
            {
                auth_token = 0;
            }
        }

        return await Task.FromResult(auth_token);

    }

    public async Task<List<ActivityLog>> GetActivityLogEntriesAsync(int UserID)
    {
        List<ActivityLog> dataList = new List<ActivityLog>(); 
        
        List<ActivityLog> al = (from logs in _context.ActivityLogs
                                where logs.UserID == UserID
                                select logs).ToList();

        return await Task.FromResult(dataList);
    }

    public async Task<List<UserProfile>> GetUserProfileAsync(int TraderID, int GroupNumber)
    {
        var profile =  (from u in _context.UserProfiles
                              where u.UserID == TraderID
                                    && u.GroupID == GroupNumber
                              select u).ToList();

        return await Task.FromResult(profile);

    }

    public async Task<List<UserProfile>> GetUserProfileListAsync(int GroupNumber)
    {
        List<UserProfile> ups = (from u in _context.UserProfiles
                                    where u.GroupID== GroupNumber
                                        select u).ToList();
        return await Task.FromResult(ups);


    }


    private async Task<int> GetNewTraderID(int GroupNumber)
    {
        bool newNumber = false;
        Random random = new Random();
        int sixDigitRandomNumber = random.Next(100000, 999999);

        while (!newNumber)
        {
            if ( (await GetUserProfileAsync(sixDigitRandomNumber,GroupNumber)).Count() == 0 ) 
                newNumber = true;  
            else
                sixDigitRandomNumber = random.Next(100000, 999999);    
        }

        return await Task.FromResult(sixDigitRandomNumber);
    }

    public async Task<int> VerifyModelTrader(NewTraderDTO user)
    {
        int rtn_val = 0;

        var traders = (from u in _context.UserProfiles
                       where u.DisplayName == user.DisplayName
                           && u.GroupID == user.GroupID
                                  select u).ToList();

        if(traders.Count() >= 1)
        {
            rtn_val =  traders[0].UserID;
        }
        
        return await Task.FromResult(rtn_val);
    }

}
