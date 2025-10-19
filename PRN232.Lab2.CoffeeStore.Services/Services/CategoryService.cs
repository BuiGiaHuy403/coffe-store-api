using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PRN232.Lab2.CoffeeStore.Repositories.Interfaces;
using PRN232.Lab2.CoffeeStore.Repositories.Models;
using PRN232.Lab2.CoffeeStore.Services.Interfaces;
using PRN232.Lab2.CoffeeStore.Services.Interfaces.Services;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Categories;
using PRN232.Lab2.CoffeeStore.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.Lab2.CoffeeStore.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        public CategoryService(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        public async Task<DataServiceResponse<Guid>> CreateCategory(CreateCategoryRequest request)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, Guid>();

            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await categoryRepo.AddAsync(newCategory);
            await _unitOfWork.SaveChangesAsync();

            return new DataServiceResponse<Guid>
            {
                Success = true,
                Message = "Category created successfully",
                Data = newCategory.Id
            };
        }

        public async Task<BaseServiceResponse> DeleteCategory(Guid categoryId)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, Guid>();

            var category = await categoryRepo.GetByIdAsync(categoryId);
            if (category == null)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            // Soft delete
            category.IsActive = false;
            category.UpdatedAt = DateTime.UtcNow;

            await categoryRepo.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new BaseServiceResponse
            {
                Success = true,
                Message = "Category deleted successfully"
            };
        }
        public async Task<DataServiceResponse<PaginationServiceResponse<object?>>> GetCategories(CategoryQueryRequest request)
        {

            var cacheKey = $"categories_cache:{request.Page}:{request.PageSize}:{request.Sort}:{request.Search}";

            if (_cache.TryGetValue(cacheKey, out DataServiceResponse<PaginationServiceResponse<object?>> cacheResponse))
                {
                return cacheResponse!;
            }

            var categoryRepo = _unitOfWork.GetRepository<Category, Guid>();
            var query = categoryRepo.Query().AsNoTracking();
            var config = new ParsingConfig { IsCaseSensitive = false };


            var entityProps = (typeof(Category)).GetProperties()
                .Select(p => p.Name.ToLower())
                .ToList();

            if(!string.IsNullOrWhiteSpace(request.Sort))
            {
                var sortFields = request.Sort.Split(',',StringSplitOptions.TrimEntries);

                var invalidSorts = sortFields
                     .Select(sf => sf.StartsWith("-") ? sf[1..] : sf)
                     .Where(sf => !entityProps.Contains(sf))
                     .ToList();
                if (invalidSorts.Any())
                {
                    return new DataServiceResponse<PaginationServiceResponse<object?>>
                    {
                        Success = false,
                        Message = $"Invalid sort fields: {string.Join(", ", invalidSorts)}",
                        Data = new PaginationServiceResponse<object?> { }
                    };
                }
            }
            if(!string.IsNullOrWhiteSpace(request.Select))
            {
                var selectFields = request.Select.Split(',', StringSplitOptions.TrimEntries);

                var invalidSelects = selectFields
                    .Where(sf => !entityProps.Contains(sf))
                    .ToList();

                if (invalidSelects.Any())
                {
                    return new DataServiceResponse<PaginationServiceResponse<object?>>
                    {
                        Success = false,
                        Message = $"Invalid select fields: {string.Join(", ", invalidSelects)}",
                        Data = new PaginationServiceResponse<object?> { }
                    };
                }

            }
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(p => p.Name.Contains(request.Search) ||
                                         p.Description.Contains(request.Search));
            }

            if (!string.IsNullOrWhiteSpace(request.Sort))
            {
                var sortFields = request.Sort.Split(',', StringSplitOptions.TrimEntries);

                var validSorts = sortFields.Select(f =>
                {
                    bool desc = f.StartsWith("-");
                    string prop = desc ? f[1..] : f;
                    var realProp = entityProps.FirstOrDefault(p =>
                       string.Equals(p, prop, StringComparison.OrdinalIgnoreCase))
                       ?? throw new Exception($"Invalid sort field: {prop}");

                    return desc ? $"{realProp} descending" : $"{realProp} ascending";
                });

                query = query.OrderBy(config, string.Join(",", validSorts));

            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            var totalCount = await query.CountAsync();
            var skip = (request.Page - 1) * request.PageSize;


            object data;

            if (!string.IsNullOrEmpty(request.Select))
            {
                var fields = request.Select.Split(',', StringSplitOptions.TrimEntries);
                var selector = "new(" + string.Join(",", fields) + ")";
                data = await query
                    .Skip(skip)
                    .Take(request.PageSize)
                    .Select(config, selector)
                    .ToDynamicListAsync();
            }
            else
            {
                data = await query
                    .Skip(skip)
                    .Take(request.PageSize)
                    .Select(c => new CategoryResponse
                    {
                        CategoryId = c.Id,
                        Name = c.Name,
                        Description = c.Description,    
                    })
                    .ToListAsync();
            }

            var result = ((IEnumerable<object>)data).ToList();

            var response = new DataServiceResponse<PaginationServiceResponse<object?>>
            {
                Success = true,
                Message = "Get Category Successfully",
                Data = new PaginationServiceResponse<object?>
                {
                    TotalCurrent = result.Count, // number of items in this page
                    TotalPage = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    Results = result!
                }
            };
            _cache.Set(cacheKey, response, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2),               
                Priority = CacheItemPriority.High
            });

            return response;

        }

        public async Task<DataServiceResponse<CategoryResponse?>> GetCategoryById(Guid categoryId)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, Guid>();
            var category = await categoryRepo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == categoryId && c.IsActive);

            if (category == null)
            {
                return new DataServiceResponse<CategoryResponse?>
                {
                    Success = false,
                    Message = "Category not found",
                    Data = null
                };
            }

            var response = new CategoryResponse
            {
                CategoryId = category.Id,
                Name = category.Name,
                Description = category.Description,
               
            };

            return new DataServiceResponse<CategoryResponse?>
            {
                Success = true,
                Message = "Category retrieved successfully",
                Data = response
            };
        }

        public async Task<BaseServiceResponse> UpdateCategory(Guid categoryId, UpdateCategoryRequest request)
        {
            var categoryRepo = _unitOfWork.GetRepository<Category, Guid>();
            var category = await categoryRepo.GetByIdAsync(categoryId);

            if (category == null)
            {
                return new BaseServiceResponse
                {
                    Success = false,
                    Message = "Category not found"
                };
            }

            if (!string.IsNullOrWhiteSpace(category.Name))
            {
                var categoryName = category.Name;
            }
            if (!string.IsNullOrEmpty(category.Description))
            {
                var categoryDescription = category.Description;
            }
            category.UpdatedAt = DateTime.UtcNow;

            await categoryRepo.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return new BaseServiceResponse
            {
                Success = true,
                Message = "Category updated successfully"
            };
        }
    }
}
