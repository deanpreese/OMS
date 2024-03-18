using OMS.Core.Models;
using OMS.Core.Common;
using Microsoft.Extensions.Logging;
using OMS.SharedKernel.DTO;


namespace OMS.Core.Interfaces;

public interface IOrderGrain  : IGrainWithStringKey
{
    Task<int> ProcessOrder(NewOrderDTO order);
}
