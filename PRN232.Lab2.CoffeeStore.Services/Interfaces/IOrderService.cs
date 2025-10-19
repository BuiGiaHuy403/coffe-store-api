using PRN232.Lab2.CoffeeStore.Repositories.Enums;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Orders;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task<DataServiceResponse<PaginationServiceResponse<OrderResponse>>> GetOrdersAsync(GetOrdersRequest request);

        Task<DataServiceResponse<OrderDetailsResponse>> GetOrderByIdAsync(Guid orderId);

        Task<DataServiceResponse<Guid>> CreateOrder(CreateOrderRequest order);

        Task<BaseServiceResponse> UpdateOrder(Guid OrderId, UpdateOrderRequest request);

        Task<BaseServiceResponse> UpdateOrderStatus(Guid OrderId, OrderStatus orderStatus);
    } 
}
