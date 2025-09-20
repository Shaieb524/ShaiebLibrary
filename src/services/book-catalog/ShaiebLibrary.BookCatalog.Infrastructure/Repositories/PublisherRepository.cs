using Microsoft.EntityFrameworkCore;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;
using ShaiebLibrary.BookCatalog.Infrastructure.Data;

namespace ShaiebLibrary.BookCatalog.Infrastructure.Repositories;

public class PublisherRepository : BaseRepository<Publisher>, IPublisherRepository
{
    public PublisherRepository(BookCatalogContext context) : base(context)
    {
    }

    public async Task<Publisher?> GetPublisherByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<Publisher>> SearchPublishersByNameAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(p => p.Name.ToLower().Contains(lowerSearchTerm))
            .ToListAsync();
    }

    public async Task<Publisher?> GetPublisherWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Books)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}