using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Model;
using Shopping.Aggregator.Services.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
#warning переделать красиво
        private readonly IBasketService _basketService;
        private readonly ICatalogService _catalogService;
        private readonly IOrderService _orderService;

        public ShoppingController(IBasketService basketService, ICatalogService catalogService, IOrderService orderService)
        {
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("{userName}", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            // get basket with username
            // iterate basket item amd consume product with basket productId member
            // map product related mambers into basketitem dto with extended columns
            // consume ordering microsevices in order to retrieve order list
            // return root ShoppingModel dto class which including all sesponses

            var basket = await _basketService.GetBasket(userName);

            foreach(var item in basket.Items)
            {
                var product = await _catalogService.GetCatalog(item.ProductId);

                // set addition products fields into basket item
                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Summary = product.Summary;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
            }

            var orders = await _orderService.GetOrdersByUserName(userName);

            var shoppingModel = new ShoppingModel
            {
                UserName = userName,
                BasketWithProduct = basket,
                Orders = orders
            };

            return Ok(shoppingModel);
        }
    }
}
