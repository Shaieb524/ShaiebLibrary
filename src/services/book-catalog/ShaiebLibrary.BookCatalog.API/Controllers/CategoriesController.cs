using Microsoft.AspNetCore.Mvc;
using ShaiebLibrary.BookCatalog.Application.DTOs.Category;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

namespace ShaiebLibrary.BookCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    /// Get all categories with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _categoryService.GetCategoriesAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving categories");
            return StatusCode(500, "An error occurred while retrieving categories");
        }
    }

    /// <summary>
    /// Get a specific category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category {CategoryId}", id);
            return StatusCode(500, "An error occurred while retrieving the category");
        }
    }

    /// <summary>
    /// Get category with subcategories
    /// </summary>
    [HttpGet("{id}/subcategories")]
    public async Task<IActionResult> GetCategoryWithSubCategories(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryWithSubCategoriesAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category with subcategories {CategoryId}", id);
            return StatusCode(500, "An error occurred while retrieving the category");
        }
    }

    /// <summary>
    /// Get category with associated books
    /// </summary>
    [HttpGet("{id}/books")]
    public async Task<IActionResult> GetCategoryWithBooks(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryWithBooksAsync(id);
            if (category == null)
                return NotFound($"Category with ID {id} not found");

            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category with books {CategoryId}", id);
            return StatusCode(500, "An error occurred while retrieving the category");
        }
    }

    /// <summary>
    /// Get root categories (categories without parent)
    /// </summary>
    [HttpGet("root")]
    public async Task<IActionResult> GetRootCategories()
    {
        try
        {
            var categories = await _categoryService.GetRootCategoriesAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving root categories");
            return StatusCode(500, "An error occurred while retrieving root categories");
        }
    }

    /// <summary>
    /// Get subcategories of a parent category
    /// </summary>
    [HttpGet("parent/{parentId}/subcategories")]
    public async Task<IActionResult> GetSubCategories(int parentId)
    {
        try
        {
            var categories = await _categoryService.GetSubCategoriesAsync(parentId);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving subcategories for parent {ParentId}", parentId);
            return StatusCode(500, "An error occurred while retrieving subcategories");
        }
    }

    /// <summary>
    /// Get category hierarchy
    /// </summary>
    [HttpGet("hierarchy")]
    public async Task<IActionResult> GetCategoryHierarchy()
    {
        try
        {
            var categories = await _categoryService.GetCategoryHierarchyAsync();
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category hierarchy");
            return StatusCode(500, "An error occurred while retrieving category hierarchy");
        }
    }

    /// <summary>
    /// Search categories by name
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchCategories([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var categories = await _categoryService.SearchCategoriesByNameAsync(searchTerm);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching categories");
            return StatusCode(500, "An error occurred while searching categories");
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating category");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category");
            return StatusCode(500, "An error occurred while creating the category");
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto updateCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.UpdateCategoryAsync(id, updateCategoryDto);
            if (category == null)
                return NotFound($"Category with ID {id} not found");

            return Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating category {CategoryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category {CategoryId}", id);
            return StatusCode(500, "An error occurred while updating the category");
        }
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound($"Category with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting category {CategoryId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category {CategoryId}", id);
            return StatusCode(500, "An error occurred while deleting the category");
        }
    }
}