using ShaiebLibrary.BookCatalog.Application.DTOs.Category;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;

namespace ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

public interface ICategoryService
{
    // Basic CRUD operations
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto?> GetCategoryWithSubCategoriesAsync(int id);
    Task<CategoryDto?> GetCategoryWithBooksAsync(int id);
    Task<PagedResult<CategoryDto>> GetCategoriesAsync(int page = 1, int pageSize = 10);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    Task<CategoryDto?> UpdateCategoryAsync(int id, CreateCategoryDto updateCategoryDto);
    Task<bool> DeleteCategoryAsync(int id);

    // Hierarchical operations
    Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentCategoryId);
    Task<IEnumerable<CategoryDto>> GetCategoryHierarchyAsync();

    // Search operations
    Task<IEnumerable<CategoryDto>> SearchCategoriesByNameAsync(string searchTerm);

    // Validation
    Task<bool> CategoryExistsAsync(int id);
    Task<bool> CanDeleteCategoryAsync(int id);
    Task<bool> IsValidParentCategoryAsync(int categoryId, int? parentCategoryId);
}