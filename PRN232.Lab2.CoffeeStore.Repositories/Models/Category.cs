using PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites;

namespace PRN232.Lab2.CoffeeStore.Repositories.Models
{
    public class Category : BaseAuditableEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public virtual List<Product>? Products { get; set; }
    }
}