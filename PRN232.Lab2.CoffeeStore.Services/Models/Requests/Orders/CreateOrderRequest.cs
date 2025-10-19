namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Orders
{
    public class CreateOrderRequest : BaseServiceRequest
    {
        public string? UserId { get; set; }

        private List<OrderItem> _orderItems = [];

        public List<OrderItem> OrderItems
        {
            get => _orderItems;
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                var dupes = value
                    .GroupBy(i => i.ProductId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (dupes.Count > 0)
                    throw new ArgumentException(
                        $"Duplicate product(s) in OrderItems: {string.Join(", ", dupes)}",
                        nameof(OrderItems));

                _orderItems = value;
            }
        }
    }
}