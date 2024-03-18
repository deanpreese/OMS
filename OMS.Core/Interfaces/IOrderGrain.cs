using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using OMS.Core.DTO;


namespace OMS.Core.Interfaces;

public interface IOrderGrain  : IGrainWithStringKey
{
    Task<int> ProcessOrder(NewOrderDTO order);
}
