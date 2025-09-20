using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Category;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;
using ShaiebLibrary.BookCatalog.Application.Extensions;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> GetCategoryWithSubCategoriesAsync(int id)
    {
        var category = await _categoryRepository.GetCategoryWithSubCategoriesAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> GetCategoryWithBooksAsync(int id)
    {
        var category = await _categoryRepository.GetCategoryWithBooksAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<PagedResult<CategoryDto>> GetCategoriesAsync(int page = 1, int pageSize = 10)
    {
        var allCategories = await _categoryRepository.GetAllAsync();
        var totalCount = allCategories.Count();
        var categories = allCategories.Skip((page - 1) * pageSize).Take(pageSize);

        return _mapper.MapToPagedResult<Category, CategoryDto>(categories, totalCount, page, pageSize);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        // Validate parent category if specified
        if (createCategoryDto.ParentCategoryId.HasValue)
        {
            var parentExists = await _categoryRepository.ExistsAsync(createCategoryDto.ParentCategoryId.Value);
            if (!parentExists)
            {
                throw new InvalidOperationException($"Parent category with ID {createCategoryDto.ParentCategoryId.Value} does not exist.");
            }
        }

        var category = _mapper.Map<Category>(createCategoryDto);
        var createdCategory = await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryDto>(createdCategory);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, CreateCategoryDto updateCategoryDto)
    {
        var existingCategory = await _categoryRepository.GetByIdAsync(id);
        if (existingCategory == null)
            return null;

        // Validate parent category if specified and different from current
        if (updateCategoryDto.ParentCategoryId.HasValue)
        {
            if (!await IsValidParentCategoryAsync(id, updateCategoryDto.ParentCategoryId.Value))
            {
                throw new InvalidOperationException("Invalid parent category assignment.");
            }
        }

        _mapper.Map(updateCategoryDto, existingCategory);
        existingCategory.UpdatedAt = DateTime.UtcNow;
        
        await _categoryRepository.UpdateAsync(existingCategory);
        return _mapper.Map<CategoryDto>(existingCategory);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        if (!await CanDeleteCategoryAsync(id))
        {
            throw new InvalidOperationException("Cannot delete category that has subcategories or associated books.");
        }

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return false;

        await _categoryRepository.DeleteAsync(category);
        return true;
    }

    public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
    {
        var categories = await _categoryRepository.GetRootCategoriesAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentCategoryId)
    {
        var categories = await _categoryRepository.GetSubCategoriesAsync(parentCategoryId);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoryHierarchyAsync()
    {
        var rootCategories = await _categoryRepository.GetRootCategoriesAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(rootCategories);
    }

    public async Task<IEnumerable<CategoryDto>> SearchCategoriesByNameAsync(string searchTerm)
    {
        var categories = await _categoryRepository.SearchCategoriesByNameAsync(searchTerm);
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        return await _categoryRepository.ExistsAsync(id);
    }

    public async Task<bool> CanDeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetCategoryWithSubCategoriesAsync(id);
        if (category == null)
            return false;

        // Check if category has subcategories
        if (category.SubCategories.Any())
            return false;

        // Check if category has associated books
        var categoryWithBooks = await _categoryRepository.GetCategoryWithBooksAsync(id);
        if (categoryWithBooks?.BookCategories.Any() == true)
            return false;

        return true;
    }

    public async Task<bool> IsValidParentCategoryAsync(int categoryId, int? parentCategoryId)
    {
        if (!parentCategoryId.HasValue)
            return true;

        // Category cannot be its own parent
        if (categoryId == parentCategoryId.Value)
            return false;

        // Parent category must exist
        if (!await _categoryRepository.ExistsAsync(parentCategoryId.Value))
            return false;

        // Check for circular reference (category cannot be parent of its ancestor)
        var parentCategory = await _categoryRepository.GetCategoryWithSubCategoriesAsync(parentCategoryId.Value);
        return !await IsDescendantOfAsync(categoryId, parentCategoryId.Value);
    }

    private async Task<bool> IsDescendantOfAsync(int ancestorId, int categoryId)
    {
        var category = await _categoryRepository.GetCategoryWithSubCategoriesAsync(categoryId);
        if (category == null)
            return false;

        foreach (var subCategory in category.SubCategories)
        {
            if (subCategory.Id == ancestorId)
                return true;

            if (await IsDescendantOfAsync(ancestorId, subCategory.Id))
                return true;
        }

        return false;
    }
}