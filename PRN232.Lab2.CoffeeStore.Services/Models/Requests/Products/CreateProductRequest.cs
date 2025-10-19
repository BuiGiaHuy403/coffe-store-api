using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Products
{
    public class CreateProductRequest : BaseServiceRequest
    {

        [Required]
        [StringLength(50)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be >=0")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductDescription { get; set; } = string.Empty;

        [Required]
        public Guid CategoryId { get; set; } = Guid.Empty;
    }
}