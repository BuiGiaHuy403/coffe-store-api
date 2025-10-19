using System.ComponentModel.DataAnnotations;

namespace PRN232.Lab2.CoffeeStore.Services.Models.Requests.Categories
{
    public class UpdateCategoryRequest
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}