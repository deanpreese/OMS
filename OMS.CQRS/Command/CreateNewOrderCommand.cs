using OMS.Core.Models;

namespace OMS.CQRS.Command;

public class CreateNewOrderCommand 
{
    public NewOrder Order { get; set; }
}
