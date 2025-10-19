using PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites;

namespace PRN232.Lab2.CoffeeStore.Repositories.Models
{
    public class OrderDetail : BaseAuditableEntity<Guid>
    {
        public Guid OrderId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }


        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}