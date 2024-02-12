
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using OMS.Core.Models;

namespace OMS.Relay.SignalHub;

public class SignalHub : Hub
{
    public async Task SendLiveOrders(List<LiveOrder> liveOrders)
    {
        await Clients.All.SendAsync("ReceiveLiveOrders", liveOrders);
    }

    public async Task SendClosedTrades(List<ClosedTrade> closedTrades)
    {
        await Clients.All.SendAsync("ReceiveClosedTrades", closedTrades);
    }
}