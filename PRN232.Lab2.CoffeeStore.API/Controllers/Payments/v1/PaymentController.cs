using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Models.Requests;
using PRN232.Lab2.CoffeeStore.Services.Interfaces;
using PRN232.Lab2.CoffeeStore.API.Mappers;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Momo;


namespace PRN232.Lab2.CoffeeStore.API.Controllers.Payments.v1
{
    [ApiController]
    [Route("/api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1)]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;
        public PaymentController(
            ILogger<PaymentController> logger,
            IPaymentService paymentService  
            ) {
            _logger = logger;
            _paymentService = paymentService;
        } 

        [HttpPost("momo/create")]
        public async Task<IActionResult> CreateMomoPayment([FromBody] MomoCreatePaymentRequest request)
        {
            var serviceResponse = await _paymentService.CreatePaymentWithMomo(request);
            if (!serviceResponse.Success)
            {
                return BadRequest(serviceResponse.ToDataApiResponse());
            }
            return Ok(serviceResponse.ToDataApiResponse());
        }

        [HttpPost("momo/confirm")]
        public async Task<IActionResult> ConfirmMomoPayment([FromBody] MomoConfirmPaymentRequest request)
        {
            var serviceResponse = await _paymentService.ConfirmMomoPaymentAsync(request);
            
            return NoContent();
        }

    }
}
