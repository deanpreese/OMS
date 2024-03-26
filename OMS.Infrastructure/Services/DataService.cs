using System;
using System.Threading.Tasks;
using OMS.Application.Interfaces;

using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;

using OMS.Application.Models;
using System.Security.Cryptography;
using OMS.Infrastructure.Interfaces;
using OMS.Application.Common;

namespace OMS.Infrastructure.Services;

public class DataService : IDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public DataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformID(int user, int group, int platform_id)
    {
        ClosedTrade closedTrade = await _unitOfWork.ClosedOrderRepository.GetLastClosedTradeByOpenPlatformID(user, group, platform_id);
        ClosedTradeDTO closedTradeDTO = null;

        if (closedTrade != null)    
        {
            try 
            {
                closedTradeDTO = await DTOMapping.MapClosedOrderToClosedOrderDTO(closedTrade);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);   
            }
        }

        return closedTradeDTO;
    }

    public async Task<ClosedTradeDTO> GetLastClosedTradeForTrader(int user, int group)
    {
        ClosedTradeDTO closedTradeDTO = null;

        List<ClosedTrade> closedTrade = await _unitOfWork.ClosedOrderRepository.Get_XXX_ClosedOrdersByTrader(user, group, 1);

        if( closedTrade.Count() > 0 )
        {
            closedTradeDTO = await DTOMapping.MapClosedOrderToClosedOrderDTO(closedTrade.FirstOrDefault());
        }
        
        return closedTradeDTO;
    }

    public async Task<List<LiveOrderDTO>> GetLiveOrdersByTrader(int user, int group)
    {
        List<LiveOrder> orders = await _unitOfWork.LiveOrderRepository.GetOrdersByTraderAsync(user, group);
        List<LiveOrderDTO> ordersDTO = new List<LiveOrderDTO>();
        foreach (LiveOrder order in orders)
        {
            ordersDTO.Add( await DTOMapping.MapOrderLiveToLiveDTO(order))  ;
        }
        return ordersDTO;
    }




}
