using Orleans;
using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using Orleans.Streams;
using OMS.Grains.Interfaces;
using Orleans.Runtime;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans.Core;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace OMS.Grains.Impl;

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
        Orders = new List<LiveOrder>();

        string[] keys = _key_string.Split('_');
        User_id = int.Parse(keys[1]);
        Group_num = int.Parse(keys[1]);

    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await UpdateProfile(_key_string);
        await UpdateScoreCard(_key_string);
        await GetLiveOrders(_key_string);
        await base.OnActivateAsync(cancellationToken);
    }


    public Task<int> GetLiveOrderCount()
    {
        return Task.FromResult(Orders.Count);
    }

    public Task<int> User_ID()
    {
        return Task.FromResult(User_id);
    }

    public Task<int> Group_Number()
    {
        return Task.FromResult(Group_num);

    }
    
    public async Task Update(string profile_key)
    {
        await UpdateProfile(_key_string);
        await UpdateScoreCard(_key_string);
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

    private async Task<UserProfile> UpdateProfile(int userId, int groupNum)
    {
        var profile_from_db = await _unitOfWork.TraderRepository.GetUserProfileAsync(userId, groupNum);
        UserProfile userProfile = new UserProfile();
        if (profile_from_db.Count > 0)
        {
            userProfile = profile_from_db.First();        
        }
       await SetProfileAsync(userProfile);
       return userProfile;
    }

    public async Task<UserProfile> UpdateProfile(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);        
        return await UpdateProfile(userId, groupNum);    
    }


    private async Task<ScoreCard> UpdateScoreCard(int userId, int groupNum)
    {
        var sc_from_db = await _unitOfWork.TraderRepository.GetScoreCardAsync(userId, groupNum);

        ScoreCard sc = new ScoreCard();
        if (sc_from_db.Count > 0)
        {
            sc = sc_from_db.First();        
        }
       await SetScoreCardAsync(sc);
       return sc;
    }

    public async Task<ScoreCard> UpdateScoreCard(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
        
        return await UpdateScoreCard(userId, groupNum);    
    }

    public async Task<List<LiveOrder>> GetLiveOrders(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
     
        Orders = await _unitOfWork.OrderRepository.GetOrdersByTraderAsync(userId, groupNum);
        return Orders;
    }

    public async Task<ClosedTrade> GetLastClosedTrade(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
        return await _unitOfWork.OrderRepository.GetLastClosedTrade(userId, groupNum);
    }


}
