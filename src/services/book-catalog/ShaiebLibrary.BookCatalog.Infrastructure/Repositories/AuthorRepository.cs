using Microsoft.EntityFrameworkCore;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;
using ShaiebLibrary.BookCatalog.Infrastructure.Data;

namespace ShaiebLibrary.BookCatalog.Infrastructure.Repositories;

public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
{
    public AuthorRepository(BookCatalogContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Author>> SearchAuthorsByNameAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(a => 
                a.FirstName.ToLower().Contains(lowerSearchTerm) ||
                a.LastName.ToLower().Contains(lowerSearchTerm) ||
                (a.FirstName + " " + a.LastName).ToLower().Contains(lowerSearchTerm))
            .ToListAsync();
    }

    public async Task<Author?> GetAuthorByFullNameAsync(string firstName, string lastName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => 
                a.FirstName.ToLower() == firstName.ToLower() && 
                a.LastName.ToLower() == lastName.ToLower());
    }

    public async Task<IEnumerable<Author>> GetAuthorsByBookAsync(int bookId)
    {
        return await _dbSet
            .Where(a => a.BookAuthors.Any(ba => ba.BookId == bookId))
            .Include(a => a.BookAuthors)
            .ToListAsync();
    }

    public async Task<Author?> GetAuthorWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(a => a.BookAuthors).ThenInclude(ba => ba.Book)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}