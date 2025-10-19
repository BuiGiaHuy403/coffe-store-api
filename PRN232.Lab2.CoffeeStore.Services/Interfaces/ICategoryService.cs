using PRN232.Lab2.CoffeeStore.Services.Models.Requests;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Categories;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;

namespace PRN232.Lab2.CoffeeStore.Services.Interfaces.Services
{
    public interface ICategoryService
    {
       
        Task<DataServiceResponse<CategoryResponse?>> GetCategoryById(Guid categoryId);

       
        Task<DataServiceResponse<Guid>> CreateCategory(CreateCategoryRequest request);

        Task<BaseServiceResponse> UpdateCategory(Guid categoryId, UpdateCategoryRequest request);

        
        Task<BaseServiceResponse> DeleteCategory(Guid categoryId);

      
        Task<DataServiceResponse<PaginationServiceResponse<object?>>> GetCategories(CategoryQueryRequest request);
    }
}
