namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Products
{
    public class GetProductsRequest : BaseServiceRequest
    {
        public int Page { get; init; } = 0;
        public int PageSize { get; init; } = 10;
        public string Name { get; init; } = string.Empty;
    }
}