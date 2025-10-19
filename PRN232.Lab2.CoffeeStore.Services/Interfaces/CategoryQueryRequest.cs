namespace PRN232.Lab2.CoffeeStore.Services.Interfaces.Services
{
    public class CategoryQueryRequest
    {
        public string? Search { get; set; } 

        public string? Select { get; set; }

        public string? Sort { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10; 

        public bool? isActive { get; set; }
    }
}