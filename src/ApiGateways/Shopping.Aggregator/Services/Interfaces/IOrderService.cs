using Shopping.Aggregator.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services.Interfaces
{
    public interface IOrderService
    {
       Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName);
    }
}
