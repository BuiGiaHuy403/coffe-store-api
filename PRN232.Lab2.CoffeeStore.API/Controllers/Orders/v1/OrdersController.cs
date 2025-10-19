using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Mappers;
using PRN232.Lab2.CoffeeStore.Repositories.Enums;
using PRN232.Lab2.CoffeeStore.Services.Interfaces;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Orders;
using System.Security.Claims;

namespace PRN232.Lab2.CoffeeStore.API.Controllers.Orders.v1
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1)]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly IOrderService _orderService;

        public OrdersController(
            ILogger<OrdersController> logger,
            IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        // POST /orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            request.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new UnauthorizedAccessException("User Not Found");

            var serviceResponse = await _orderService.CreateOrder(request);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.ToBaseApiResponse());

            return Created($"/api/v1/orders/{serviceResponse.Data}", serviceResponse.ToDataApiResponse());
        }

        // GET /orders
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] GetOrdersRequest request)
        {
            
            request.UserId ??= User.FindFirstValue(ClaimTypes.NameIdentifier);

            var serviceResponse = await _orderService.GetOrdersAsync(request);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.ToBaseApiResponse());

            return Ok(serviceResponse.ToDataApiResponse());
        }

        // GET /orders/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var serviceResponse = await _orderService.GetOrderByIdAsync(id);

            if (!serviceResponse.Success)
                return NotFound(serviceResponse.ToBaseApiResponse());

            return Ok(serviceResponse.ToDataApiResponse());
        }

        // PUT /orders/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderRequest request)
        {
            var serviceResponse = await _orderService.UpdateOrder(id, request);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.ToBaseApiResponse());

            return Ok(serviceResponse.ToBaseApiResponse());
        }

        // PATCH /orders/{id}/status
        [HttpPatch("{id:guid}/checkout")]
        public async Task<IActionResult> Checkout(Guid id)
        {
            var serviceResponse = await _orderService.UpdateOrderStatus(id, OrderStatus.Pending);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.ToBaseApiResponse());

            return Ok(serviceResponse.ToBaseApiResponse());
        }

        // POST /orders/{id}/complete
        [HttpPatch("{id:guid}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            var serviceResponse = await _orderService.UpdateOrderStatus(id, OrderStatus.Completed);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.ToBaseApiResponse());

            return Ok(serviceResponse.ToBaseApiResponse());
        }

        // POST /orders/{id}/cancel
        [HttpPatch("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var serviceResponse = await _orderService.UpdateOrderStatus(id, OrderStatus.Cancelled);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse.ToBaseApiResponse());

            return Ok(serviceResponse.ToBaseApiResponse());
        }
    }
}
