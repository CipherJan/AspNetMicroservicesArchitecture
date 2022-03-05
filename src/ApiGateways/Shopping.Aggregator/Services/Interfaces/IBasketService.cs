using Shopping.Aggregator.Model;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Services.Interfaces
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
    }
}
