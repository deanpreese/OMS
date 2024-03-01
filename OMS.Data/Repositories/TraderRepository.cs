using OMS.Core.Interfaces;
using OMS.Core.Models;
using OMS.Core.Common;

namespace OMS.Data;

public class TraderRepository : ITraderRepository
{
    private readonly OrderManagementDbContext _context;

    public TraderRepository(OrderManagementDbContext context)
    {
        _context = context;
    }

    public Task AddLogEntryAsync(ActivityLog log)
    {
        _context.ActivityLogs.Add(log);
        return Task.CompletedTask;
    }

    public Task<int> AddTraderAsync(NewTrader addedTrader)
    {
        UserProfile user = new UserProfile();    
        user.UserID =  GetNewTraderID(addedTrader.GroupID);
        user.DisplayName = addedTrader.DisplayName;
        user.UserPwd = addedTrader.UserPwd;
        user.Email = addedTrader.Email;
        user.FirstName = addedTrader.FirstName;
        user.LastName = addedTrader.LastName;
        user.DateRegistered = DateTime.Now;
        user.Enabled = 1;
        user.EnabledLive = 0;
        user.Leverage = 1;
        user.IsOpposite = 0;
        user.GroupRank = 0;
        user.GroupID = addedTrader.GroupID;
        user.TraderRole = 1;
        
        ScoreCard scd = new ScoreCard();
        scd.UserID  = user.UserID;
        scd.TradeXML = " ";
        scd.GroupID = addedTrader.GroupID;
        _context.Add(scd);
        _context.UserProfiles.Add(user);
        return Task.FromResult(user.UserID);

    }

    public Task<int> AuthenticateTraderAsync(int userID, string password, int groupNumber)
    {
        Guid g = Guid.NewGuid();
        byte[] gb = g.ToByteArray();
        int auth_token = Math.Abs(BitConverter.ToInt32(gb, 0));

        List<UserProfile> userList = GetUserProfile(userID ,groupNumber);

        if (userList.Count > 0)
        {
            UserProfile u = userList.FirstOrDefault();
            if (u != null && u.UserPwd != null && u.UserPwd != password)
            {
                auth_token = 0;
            }
        }

        return Task.FromResult(auth_token);

    }

    public Task<List<ActivityLog>> GetActivityLogEntriesAsync(int UserID)
    {
        List<ActivityLog> dataList = new List<ActivityLog>(); 
        
        List<ActivityLog> al = (from logs in _context.ActivityLogs
                                where logs.UserID == UserID
                                select logs).ToList();

        return Task.FromResult(dataList);
    }

    public Task<List<UserProfile>> GetUserProfileAsync(int TraderID, int GroupNumber)
    {
        var profile =  (from u in _context.UserProfiles
                              where u.UserID == TraderID
                                    && u.GroupID == GroupNumber
                              select u).ToList();

        return Task.FromResult(profile);

    }

    public Task<List<UserProfile>> GetUserProfileListAsync(int GroupNumber)
    {
        List<UserProfile> ups = (from u in _context.UserProfiles
                                    where u.GroupID== GroupNumber
                                        select u).ToList();
        return Task.FromResult(ups);


    }

    public Task<List<ScoreCard>> GetScoreCardAsync(int TraderID , int GroupNumber)
    {
        var scorecard = (from sc in _context.ScoreCard
                            where sc.UserID == TraderID
                                && sc.GroupID == GroupNumber
                            select sc).ToList();

        return Task.FromResult(scorecard);
    }


    public List<UserProfile> GetUserProfile(int TraderID , int GroupNumber)
    {
        return (from u in _context.UserProfiles
                            where u.UserID == TraderID
                                && u.GroupID == GroupNumber
                            select u).ToList();
    }

    private int GetNewTraderID(int GroupNumber)
    {
        bool newNumber = false;
        Random random = new Random();
        int sixDigitRandomNumber = random.Next(100000, 999999);

        while (!newNumber)
        {
            if ( GetUserProfile(sixDigitRandomNumber,GroupNumber).Count() == 0 ) 
                newNumber = true;  
            else
                sixDigitRandomNumber = random.Next(100000, 999999);    
        }

        return sixDigitRandomNumber;
    }

    public async Task<int> VerifyModelTrader(NewTrader user)
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
