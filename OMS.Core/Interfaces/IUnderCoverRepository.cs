using OMS.Core.Models;

namespace OMS.Core.Interfaces;

public interface IUnderCoverRepository
{
    Task AddToOrderFlowAsync(LiveOrder o);
    Task AddToActivityLog(ActivityLog log);
}
