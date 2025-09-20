using ShaiebLibrary.BookCatalog.Domain.Entities;

namespace ShaiebLibrary.BookCatalog.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId);
    Task<Category?> GetCategoryWithSubCategoriesAsync(int id);
    Task<Category?> GetCategoryWithBooksAsync(int id);
    Task<IEnumerable<Category>> SearchCategoriesByNameAsync(string searchTerm);
}