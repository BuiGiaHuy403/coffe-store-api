using PRN232.Lab2.CoffeeStore.Repositories.Enums;
using PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites;

namespace PRN232.Lab2.CoffeeStore.Repositories.Models
{
    public class Payment : BaseAuditableEntity<Guid>
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        // Navigation property
        public virtual Order Order { get; set; } = null!;
    }
}