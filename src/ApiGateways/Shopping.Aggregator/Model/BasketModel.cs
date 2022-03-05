using System.Collections.Generic;

namespace Shopping.Aggregator.Model
{
    public class BasketModel
    {
        public string UserName { get; set; }
        public List<BasketItemExtendedModel> Item { get; set; } = new List<BasketItemExtendedModel>();
        public decimal TotalPrice { get; set; }
    }
}
