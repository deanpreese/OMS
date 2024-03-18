using OMS.Core.Models;
using OMS.SharedKernel.DTO;
using OMS.SharedKernel.Grains;
using OMS.Core.Interfaces;
using OMS.Core.Common;


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

    public async  Task<List<LiveOrderDTO>> GetLiveOrders(string profile_key)
    {
         string[] profile_key_parts = profile_key.Split('_');
        int userId = int.Parse(profile_key_parts[0]);
        int groupNum = int.Parse(profile_key_parts[1]);
     
        List<LiveOrder> orders = await _unitOfWork.LiveOrderRepository.GetOrdersByTraderAsync(userId, groupNum);

        List<LiveOrderDTO> ordersDTO = new List<LiveOrderDTO>();
        foreach (LiveOrder order in orders)
        {
            ordersDTO.Add( await DTOMapping.MapOrderLiveToLiveDTO(order))  ;
        }
        return ordersDTO;
    }
}
