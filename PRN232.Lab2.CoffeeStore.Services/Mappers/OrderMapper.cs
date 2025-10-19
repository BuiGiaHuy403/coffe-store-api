using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Orders;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Mappers
{
    public static class OrderMapper
    {
        public static OrderResponse ToOrderResponse(this Order order)
        {
            return new OrderResponse
            {
                OrderId = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice),
                TotalItems = order.OrderDetails.Sum(od => od.Quantity),
                PaymentId = order.PaymentId
            };
        }

        public static OrderDetailsResponse ToOrderDetailsResponse(this Order order)
        {
            return new OrderDetailsResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.OrderDetails.Sum(od => od.Quantity * od.UnitPrice),
                Payment = order.Payment.ToPaymentResponse(),
                OrderItems = order.OrderDetails.Select(od => od.ToOrderItemResponse()).ToList()
            };
        }

        public static OrderItemResponse ToOrderItemResponse(this OrderDetail orderDetail)
        {
            return new OrderItemResponse
            {
                Id = orderDetail.Id,
                ProductId = orderDetail.ProductId,
                ProductName = orderDetail.Product?.Name ?? "",
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
                TotalPrice = orderDetail.Quantity * orderDetail.UnitPrice
            };
        }

        public static void UpdateOrder(this Order order, UpdateOrderRequest request)
        {
            order.UpdatedAt = DateTime.UtcNow;
        }
    }
}
