using PRN232.Lab2.CoffeeStore.Repositories.Enums;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Orders
{
    public class GetOrderRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? UserId { get; set; }

        public OrderStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}