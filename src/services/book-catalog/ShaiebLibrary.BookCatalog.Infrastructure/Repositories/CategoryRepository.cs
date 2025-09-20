using Microsoft.EntityFrameworkCore;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;
using ShaiebLibrary.BookCatalog.Infrastructure.Data;

namespace ShaiebLibrary.BookCatalog.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(BookCatalogContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.ParentCategoryId == null)
            .Include(c => c.SubCategories)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId)
    {
        return await _dbSet
            .Where(c => c.ParentCategoryId == parentCategoryId)
            .Include(c => c.SubCategories)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryWithSubCategoriesAsync(int id)
    {
        return await _dbSet
            .Include(c => c.SubCategories)
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetCategoryWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(c => c.BookCategories).ThenInclude(bc => bc.Book)
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Category>> SearchCategoriesByNameAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(c => 
                c.Name.ToLower().Contains(lowerSearchTerm) ||
                c.Description!.ToLower().Contains(lowerSearchTerm))
            .Include(c => c.ParentCategory)
            .ToListAsync();
    }
}