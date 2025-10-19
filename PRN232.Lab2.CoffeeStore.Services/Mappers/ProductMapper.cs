using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Products;

namespace PRN232.Lab2.CoffeeStore.Services.Mappers;

public static class ProductMapper
{
    public static ProductResponse ToProductResponse(this Product product)
    {
        return new ProductResponse()
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            Category = product.Category!.ToCategoryResponse()
        };
    }

    public static Product ToProduct(this CreateProductRequest request)
    {
        return new Product()
        {
            Id = Guid.NewGuid(),
            Name = request.ProductName,
            Price = request.Price,
            Description = request.ProductDescription,
            CategoryId = request.CategoryId
        };
    }

    public static void UpdateProduct(this Product product, UpdateProductRequest request)
    {
        product.Name = request.Name;
        product.Price = request.Price;
        product.Description = request.Description;
        product.CategoryId = request.CategoryId;
    }
}