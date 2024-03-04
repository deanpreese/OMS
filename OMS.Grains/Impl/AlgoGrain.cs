using OMS.Core.Models;
using OMS.Grains.Interfaces;
using OMS.Core.Interfaces;


namespace OMS.Grains.Impl;

public class AlgoGrain : Grain, IAlgoGrain
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _key_string;
    public int Algo_User_id { get; set; }
    public int Algo_Group_num { get; set; }
    public List<LiveOrder> OpenOrders { get; set; }     
    
    public AlgoGrain( IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _key_string = this.GetPrimaryKeyString();
        OpenOrders = new List<LiveOrder>();

        string[] keys = _key_string.Split('_');
        Algo_User_id = int.Parse(keys[1]);
        Algo_Group_num = int.Parse(keys[1]);

    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await GetLiveOrders(_key_string);
        await GetLastClosedTrade(_key_string);
        await GetLiveOrders(_key_string);
        await base.OnActivateAsync(cancellationToken);
    }

   

    public Task<int> Algo_Group_Number()
    {
        return Task.FromResult(Algo_Group_num);
    }

    public Task<int> Algo_ID()
    {
        return Task.FromResult(Algo_User_id);
    }

    public async Task<ClosedTrade> GetLastClosedTrade(string profile_key)
    {
        string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
        return await _unitOfWork.OrderRepository.GetLastClosedTrade(userId, groupNum);
    }

    public Task<int> GetLiveOrderCount()
    {
        return Task.FromResult(OpenOrders.Count);
    }

    public async  Task<List<LiveOrder>> GetLiveOrders(string profile_key)
    {
         string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
     
        OpenOrders = await _unitOfWork.OrderRepository.GetOrdersByTraderAsync(userId, groupNum);
        return OpenOrders;
    }
}
