using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpsServices;
using Basket.API.Repositories;
using EnentBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcServices;
        private readonly IPublishEndpoint _publishEndpoint;
        private IMapper _mapper;

        public BasketController(IBasketRepository repository,
                DiscountGrpcService discountGrpcServices, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountGrpcServices = discountGrpcServices ?? throw new ArgumentNullException(nameof(repository));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            // Communicate with Discount.gRPC
            // Calcelate latest prices of product into shopping cart
            // Consume Discount Grpc
            foreach (var item in basket.Items)
            {
                var coupon = await _discountGrpcServices.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            var deleteResult = await _repository.UpdateBasket(basket);
            return Ok(deleteResult);
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // Get existing basket with total price
            // Create basketCheckoutEvent -- Set TotalPrice on basketCheckout eventMessage
            // Send checkout event to rabbitmq
            // Remove the basket

            var basket = await _repository.GetBasket(basketCheckout.UserName);
            if (basket == null)
                return BadRequest();

            //send checkout event to rabbitmq
            var eventMassage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMassage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMassage);

            await _repository.DeleteBasket(basket.UserName);
            return Accepted();
        }
    }
}
