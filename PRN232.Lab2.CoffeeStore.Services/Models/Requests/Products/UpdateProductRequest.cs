using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Products
{
    public class UpdateProductRequest : BaseServiceRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0.")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        public Guid CategoryId { get; set; }
    }
}