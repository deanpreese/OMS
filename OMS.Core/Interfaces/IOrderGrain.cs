using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;


namespace OMS.Core.Interfaces;

public interface IOrderGrain  : IGrainWithStringKey
{
    Task<int> ProcessOrder(NewOrder order);
}
