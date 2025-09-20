using ShaiebLibrary.BookCatalog.Application.DTOs.Common;

namespace ShaiebLibrary.BookCatalog.Application.DTOs.Category;

public class CategoryDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public IEnumerable<CategoryDto> SubCategories { get; set; } = new List<CategoryDto>();
}