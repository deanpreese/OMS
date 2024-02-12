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
using OMS.Services.Queue;

namespace OMS.Grains.Impl;


public class OrderGrain : Grain, IOrderGrain
{

    private IUnitOfWork _unitOfWork;
    private readonly NewOrderChannelService _newOrderChannelService;

    public OrderGrain(IUnitOfWork unitOfWork, NewOrderChannelService newOrderChannelService)
    {
        _unitOfWork = unitOfWork;
        _newOrderChannelService = newOrderChannelService;
    }


    public async Task<int> ProcessMTOrder(NewOrder order)
    {

        int auth_code =  await _unitOfWork.TraderRepository.AuthenticateTraderAsync(order.UserID, "abc", order.UserGroup);
        if( auth_code == 0 )
        {
            // Need to add trader
            NewTrader newTrader = new NewTrader
            {
                Group = order.UserGroup,
                UserId = 0,
                DisplayName = order.UserName,
                UserPwd = "abc",
                FirstName = "MetaTrader",
                LastName = "EA",
                Email = "abc.xyz"

            };

            int trader_id = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            auth_code = await _unitOfWork.TraderRepository.AuthenticateTraderAsync(trader_id, "abc", order.UserGroup);
        }

        int om_id = 0;
        if (auth_code != 0)
        {
            try
            {
                await _newOrderChannelService.WriteAsync(order);
                om_id = 87654321;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        return om_id;
    }

    public Task<int> ProcessOrder(NewOrder order)
    {
        throw new NotImplementedException();
    }
}
