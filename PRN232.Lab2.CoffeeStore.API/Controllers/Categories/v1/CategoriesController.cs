using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.API.Mappers;
using PRN232.Lab2.CoffeeStore.Services.Infrastructure;
using PRN232.Lab2.CoffeeStore.Services.Interfaces.Services;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests;
using PRN232.Lab2.CoffeeStore.Services.Models.Requests.Categories;
using System.Diagnostics;

namespace PRN232.Lab2.CoffeeStore.API.Controllers.Categories.v1;

[ApiVersion(1)]
[ApiVersion(2)]
[ApiController]
[Route("api/v{version:apiVersion}/categories")]
//[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;
    public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

  
    /// <summary>
    /// Get categories with advanced query (sort, select, paging) (v2)
    /// </summary>
    [MapToApiVersion(1)]
    [HttpGet("")]
    public async Task<IActionResult> GetCategoriesAsync([FromQuery] CategoryQueryRequest request)
    {
        var stopwatch = Stopwatch.StartNew();

        var serviceResponse = await _categoryService.GetCategories(request);
        stopwatch.Stop();
        _logger.LogWarning("Request for took {ElapsedMilliseconds} ms", stopwatch.ElapsedMilliseconds);
        if (!serviceResponse.Success)
        {
            return BadRequest(serviceResponse.ToBaseApiResponse());
        }
        return Ok(serviceResponse.ToDataApiResponse(Request, Response));
    }

    /// <summary>
    /// Get category by Id
    /// </summary>
    [MapToApiVersion(1)]
    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] Guid categoryId, [FromQuery] BaseServiceRequest request)
    {
        var serviceResponse = await _categoryService.GetCategoryById(categoryId);
        if (!serviceResponse.Success)
        {
            return NotFound(serviceResponse.ToDataApiResponse());
        }
        return Ok(serviceResponse.ToDataApiResponse());
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [MapToApiVersion(1)]
    [HttpPost("")]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CreateCategoryRequest request)
    {
        var serviceResponse = await _categoryService.CreateCategory(request);

        if (!serviceResponse.Success)
        {
            return BadRequest(serviceResponse.ToDataApiResponse());
        }

        return Created($"/api/v1/categories/{serviceResponse.ToDataApiResponse().Data}", null);
    }

    /// <summary>
    /// Update category by Id
    /// </summary>
    [MapToApiVersion(1)]
    [HttpPut("{categoryId}")]
    public async Task<IActionResult> UpdateCategoryAsync(Guid categoryId, [FromBody] UpdateCategoryRequest request)
    {
        var serviceResponse = await _categoryService.UpdateCategory(categoryId, request);

        if (!serviceResponse.Success)
        {
            return BadRequest(serviceResponse.ToBaseApiResponse());
        }

        return NoContent();
    }

    /// <summary>
    /// Soft delete a category by Id
    /// </summary>
    [MapToApiVersion(1)]
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteCategoryAsync(Guid categoryId)
    {
        var serviceResponse = await _categoryService.DeleteCategory(categoryId);

        if (!serviceResponse.Success)
        {
            return NotFound(serviceResponse.ToBaseApiResponse());
        }

        return NoContent();
    }
}
