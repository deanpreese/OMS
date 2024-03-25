using OMS.Application.Models;

namespace OMS.Application.Interfaces;

public interface IUnderCoverRepository
{
    Task AddToOrderFlowAsync(LiveOrder o);
    Task AddToActivityLog(ActivityLog log);
}
