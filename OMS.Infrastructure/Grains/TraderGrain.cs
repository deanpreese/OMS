using OMS.Core.Models;

using Orleans.Runtime;
using OMS.Core.Interfaces;

namespace OMS.Infrastructure.Grains;

public class TraderGrain : Grain, ITraderGrain
{
    private readonly IPersistentState<UserProfile> _profile;
    private readonly IPersistentState<ScoreCard> _scoreCard;
    
    private string _key_string ;
    private IUnitOfWork _unitOfWork;

    public List<LiveOrder> Orders;
    public int User_id { get; set; }
    public int Group_num { get; set; }

    public TraderGrain(
        [PersistentState("profile", "GrainStorage")]
        IPersistentState<UserProfile> profile, 
        [PersistentState("scorecard", "GrainStorage")]
        IPersistentState<ScoreCard> scoreCard,
         IUnitOfWork unitOfWork)
    {
        _profile = profile;
        _unitOfWork = unitOfWork;
        _scoreCard = scoreCard;
        _key_string = this.GetPrimaryKeyString();


    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await UpdateProfile(_key_string);
        await UpdateScoreCard(_key_string);
        await base.OnActivateAsync(cancellationToken);
    }


    public async Task SetProfileAsync(UserProfile profile_to_set)
    {
        _profile.State = profile_to_set;
        await _profile.WriteStateAsync();
    }

    public async Task SetScoreCardAsync(ScoreCard scoreCard_to_set)
    {
        _scoreCard.State = scoreCard_to_set;
        await _scoreCard.WriteStateAsync();
    }

    public async Task<UserProfile> GetProfileAsync(string profile_key){
        return await UpdateProfile(profile_key);
    } 
    public async Task<ScoreCard> GetScoreCardAsync(string profile_key){
        return await UpdateScoreCard(profile_key);
    }

    public async Task<UserProfile> UpdateProfile(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]); 

         var profile_from_db = await _unitOfWork.TraderRepository.GetUserProfileAsync(userId, groupNum);
        UserProfile userProfile = new UserProfile();
        if (profile_from_db.Count > 0)
        {
            userProfile = profile_from_db.First();        
        }
       await SetProfileAsync(userProfile);
       return userProfile;
    }


    public async Task<ScoreCard> UpdateScoreCard(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
        
        var sc_from_db = await _unitOfWork.TraderRepository.GetScoreCardAsync(userId, groupNum);

        ScoreCard sc = new ScoreCard();
        if (sc_from_db.Count > 0)
        {
            sc = sc_from_db.First();        
        }
       await SetScoreCardAsync(sc);
       return sc;
    }

    public async Task<List<LiveOrder>> GetLiveOrders(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
     
        Orders = await _unitOfWork.OrderRepository.GetOrdersByTraderAsync(userId, groupNum);
        return Orders;
    }

    public async Task<ClosedTrade> GetLastClosedTradeByOpenPlatformID(string profile_key, int platform_id)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
        return await _unitOfWork.OrderRepository.GetLastClosedTradeByOpenPlatformID(userId, groupNum, platform_id);
    }

}
