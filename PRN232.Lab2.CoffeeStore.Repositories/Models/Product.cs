using PRN232.Lab2.CoffeeStore.Repositories.Models.BaseEnitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Repositories.Models
{
    public class Product : BaseAuditableEntity<Guid>
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public Guid CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}
