namespace PRN232.Lab2.CoffeeStore.API.Models.Responses
{
    public class DataApiResponse<T>: BaseApiResponse
    {
        public T Data { get; set; } = default!;
    }
}
