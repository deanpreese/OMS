using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using Orleans.Streams;


namespace OMS.Grains.Interfaces;

public interface IOrderGrain  : IGrainWithStringKey
{
    Task<int> ProcessOrder(NewOrder order);
}
