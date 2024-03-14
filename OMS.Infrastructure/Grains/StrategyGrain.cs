using OMS.Core.Models;

using OMS.Core.Interfaces;


namespace OMS.Infrastructure.Grains;

public class StrategyGrain : Grain, IStrategyGrain
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _key_string;
    
    public StrategyGrain( IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _key_string = this.GetPrimaryKeyString();
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await GetLiveOrders(_key_string);
        await base.OnActivateAsync(cancellationToken);
    }

    public async  Task<List<LiveOrder>> GetLiveOrders(string profile_key)
    {
         string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
     
        List<LiveOrder> orders = await _unitOfWork.LiveOrderRepository.GetOrdersByTraderAsync(userId, groupNum);
        return orders;
    }
}
