using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Products;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Infrastructure
{
    public interface IProductService
    {
        Task<DataServiceResponse<PaginationServiceResponse<ProductResponse>>> GetProducts(GetProductsRequest request);
        Task<DataServiceResponse<ProductResponse?>> GetProductById(Guid productId);
        Task<DataServiceResponse<Guid>> CreateProduct(CreateProductRequest request);
        Task<BaseServiceResponse> UpdateProduct(Guid productId, UpdateProductRequest request);
        Task<BaseServiceResponse> DeleteProduct(Guid productId);


        Task<DataServiceResponse<PaginationServiceResponse<object?>>>
            GetProducts(ProductQueryRequest request);
    }
}
