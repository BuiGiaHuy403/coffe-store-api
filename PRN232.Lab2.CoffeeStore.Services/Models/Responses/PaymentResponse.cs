namespace PRN232.Lab2.CoffeeStore.Services.Models.Responses
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = null!;
    }
}