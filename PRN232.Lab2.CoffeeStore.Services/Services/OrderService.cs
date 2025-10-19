using Azure.Core;
using Microsoft.EntityFrameworkCore;
using PRN232.Lab2.CoffeeStore.Repositories.Enums;
using PRN232.Lab2.CoffeeStore.Repositories.Interfaces;
using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Interfaces;
using PRN232.Lab2.CoffeeStore.Services.Mappers;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Orders;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderService(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }
        public async Task<DataServiceResponse<Guid>> CreateOrder(CreateOrderRequest request)
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var productRepo = _unitOfWork.GetRepository<Product, Guid>();

            var productIds = request.OrderItems.Select(x => x.ProductId).ToList();

            var existingProducts = await productRepo.Query()
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToListAsync();

            var missingIds = productIds.Except(existingProducts.Select(e => e.Id)).ToList();

            if (missingIds.Any())
            {
                return new DataServiceResponse<Guid>()
                {
                    Success = false,
                    Message = $"Products not found or inactive: {string.Join(", ", missingIds)}",
                    Data = Guid.Empty
                };
            }

            // map order
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId!, 
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            var orderDetails = request.OrderItems.Select(oi =>
            {
                var product = existingProducts.First(p => p.Id == oi.ProductId);
                return new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = newOrder.Id,
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = product.Price,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }).ToList();
            newOrder.OrderDetails = orderDetails;
            await orderRepo.AddAsync(newOrder);
            await _unitOfWork.SaveChangesAsync();

            return new DataServiceResponse<Guid>()
            {
                Success = true,
                Message = "Order created successfully",
                Data = newOrder.Id
            };
        }

        public async Task<DataServiceResponse<OrderDetailsResponse>> GetOrderByIdAsync(Guid orderId)
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();

            var order = await orderRepo.Query()
                .Where(o => o.IsActive)
                .Include(o => o.OrderDetails.Where(od => od.IsActive))
                .   ThenInclude(od => od.Product)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new DataServiceResponse<OrderDetailsResponse>
                {
                    Success = false,
                    Message = "Order not found",
                    Data = null!
                };
            }

            var response = order.ToOrderDetailsResponse();



            return new DataServiceResponse<OrderDetailsResponse>
            {
                Success = true,
                Message = "Order retrieved successfully",
                Data = response
            };
        }

        public async Task<BaseServiceResponse> UpdateOrder(Guid orderId, UpdateOrderRequest request)
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var order = await orderRepo.Query()
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = "Order not found"
                };
            }

            // Xóa OrderDetails cũ, thay bằng mới
            order.OrderDetails.Clear();
            foreach (var item in request.OrderItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }

            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new BaseServiceResponse
            {
                Success = true,
                Message = "Order updated successfully"
            };
        }

        public async Task<DataServiceResponse<PaginationServiceResponse<OrderResponse>>> GetOrdersAsync(GetOrdersRequest request)
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();

            var query = orderRepo.Query().AsQueryable();

            // Filter theo UserId (nếu có)
            if (!string.IsNullOrEmpty(request.UserId))
            {
                query = query.Where(o => o.UserId == request.UserId);
            }

            // Filter theo Status (nếu có)
            if (request.Status.HasValue)
            {
                query = query.Where(o => o.Status == request.Status.Value);
            }

            // Filter theo Date range
            if (request.FromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= request.ToDate.Value);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderResponse
                {
                    OrderId = o.Id,
                    UserId = o.UserId,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.OrderDetails.Sum(d => d.Quantity * d.Product.Price)
                })
                .ToListAsync();

            return new DataServiceResponse<PaginationServiceResponse<OrderResponse>>
            {
                Success = true,
                Message = "Orders retrieved successfully",
                Data = new PaginationServiceResponse<OrderResponse>
                {
                    TotalCurrent = orders.Count,
                    TotalPage = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    Results = orders
                }
            };
        }

        public async Task<BaseServiceResponse> UpdateOrderStatus(Guid orderId, OrderStatus orderStatus)
        {
            var orderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var order = await orderRepo.GetByIdAsync(orderId);

            if (order == null)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = "Order not found"
                };
            }

            order.Status = orderStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return new BaseServiceResponse
            {
                Success = true,
                Message = $"Order status updated to {orderStatus}"
            };
        }
    }
}
