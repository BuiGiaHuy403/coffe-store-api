namespace PRN232.Lab2.CoffeeStore.API.Models.Responses
{
    public class BaseApiResponse
    {
        public string Message { get; set; } = null!;

        public bool Success { get; set; }
    }
}
