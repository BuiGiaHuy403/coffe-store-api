using PRN232.Lab2.CoffeeStore.Repositories.Enums;
using PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites;

namespace PRN232.Lab2.CoffeeStore.Repositories.Models
{
    public class Order : BaseAuditableEntity<Guid>
    {
        public string UserId { get; set; } = null!;

        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        public Guid? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}