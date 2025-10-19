using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PRN232.Lab2.CoffeeStore.API.Models.Requests;
using PRN232.Lab2.CoffeeStore.API.Models.Responses;
using PRN232.Lab2.CoffeeStore.Repositories.Enums;
using PRN232.Lab2.CoffeeStore.Repositories.Interfaces;
using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Interfaces;
using PRN232.Lab2.CoffeeStore.Services.Mappers;
using PRN232.Lab2.CoffeeStore.Services.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Momo;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System.Text;
using Formatting = Newtonsoft.Json.Formatting;

namespace PRN232.Lab2.CoffeeStore.Services.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;

        private readonly HttpClient _client;

        private readonly MomoSettings _momoSettings;

        private readonly  IUnitOfWork _unitOfWork;
        public PaymentService(ILogger<PaymentService> logger, HttpClient client, IOptions<MomoSettings> options, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _client = client;
            _momoSettings = options.Value;
            _unitOfWork = unitOfWork;
        }

        public async Task<DataServiceResponse<MomoCreatePaymentResponse>> CreatePaymentWithMomo(MomoCreatePaymentRequest request)
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();


            var order = await orderRepo.Query()
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);

            if (order is null)
            {
                return new DataServiceResponse<MomoCreatePaymentResponse>
                {
                    Message = $"Order not found with order id : {request.OrderId}",
                    Success = false,
                    Data = null!,
                };
            }

            var amount = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice);


            var momoRequest = request.ToMomoOneTimePayment(_momoSettings,(long)amount);


            var requestJson = JsonConvert.SerializeObject(momoRequest, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
            });
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
           
            _logger.LogInformation("POST to URL: {Url}", _momoSettings.PaymentUrl);

            var response = await _client.PostAsync(_momoSettings.PaymentUrl, content);

            var responseJson = await response.Content.ReadAsStringAsync();
            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponse>(responseJson);
            _logger.LogInformation("MoMo Raw Response: {Response}", responseJson);
            _logger.LogInformation("MoMo Status Code: {StatusCode}", response.StatusCode);

            return new DataServiceResponse<MomoCreatePaymentResponse>
            {
                Message = "Get momo payment link successfully",
                Success = true,
                Data = momoResponse!,
            }; 


        }

        public async Task<BaseServiceResponse> ConfirmMomoPaymentAsync(MomoConfirmPaymentRequest request)
        {
            _logger.LogInformation("Confirming MoMo payment: {Request}",
                JsonConvert.SerializeObject(request, Formatting.Indented));

            if (!request.IsValidSignature(_momoSettings))
            {
                _logger.LogError("Invalid signature for MoMo confirm");
                return new BaseServiceResponse
                {
                    Message = "Error while confirm payment",
                    Success = false
                };
            }

            var paymentRepo = _unitOfWork.GetRepository<Payment, Guid>();
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();

            var order = await orderRepo.GetByIdAsync(Guid.Parse(request.OrderId!));

            if (order == null)
            {
                _logger.LogError("Order not found for OrderId {OrderId}", request.OrderId);
                return new BaseServiceResponse
                {
                    Message = "Error while confirm payment",
                    Success = false
                };
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Amount = request.Amount ?? 0,
                PaymentDate = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await paymentRepo.AddAsync(payment);
            order.Status = OrderStatus.Completed; 
            order.PaymentId = payment.Id;
            order.UpdatedAt = DateTime.UtcNow;
            await orderRepo.UpdateAsync(order);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} confirmed. Order {OrderId} status = {Status}",
                payment.Id, order.Id, order.Status);

             return new BaseServiceResponse
            {
                Message = "Confirm Payment Successfully",
                Success = true,
            };
        }


    }
}
