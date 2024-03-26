using System;
using System.Threading.Tasks;
using OMS.Application.Interfaces;
using OMS.Application.Models;
using System.Security.Cryptography;
using OMS.SharedKernel.Common;
using OMS.SharedKernel.DTO;



namespace OMS.Infrastructure.Interfaces;


public interface IDataService
{
    Task<List<LiveOrderDTO>> GetLiveOrdersByTrader(int user, int group);
    Task<ClosedTradeDTO> GetLastClosedTradeByOpenPlatformID(int user, int group, int platform_id);
    Task<ClosedTradeDTO> GetLastClosedTradeForTrader(int user, int group);
}
