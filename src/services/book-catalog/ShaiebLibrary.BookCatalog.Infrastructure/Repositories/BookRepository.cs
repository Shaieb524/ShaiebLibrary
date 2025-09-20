using Microsoft.EntityFrameworkCore;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Enums;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;
using ShaiebLibrary.BookCatalog.Infrastructure.Data;

namespace ShaiebLibrary.BookCatalog.Infrastructure.Repositories;

public class BookRepository : BaseRepository<Book>, IBookRepository
{
    public BookRepository(BookCatalogContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Book>> GetBooksByLanguageAsync(Language language)
    {
        return await _dbSet
            .Where(b => b.Language == language)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByStatusAsync(BookStatus status)
    {
        return await _dbSet
            .Where(b => b.Status == status)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
    {
        return await _dbSet
            .Where(b => b.BookAuthors.Any(ba => ba.AuthorId == authorId))
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId))
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByPublisherAsync(int publisherId)
    {
        return await _dbSet
            .Where(b => b.PublisherId == publisherId)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<Book?> GetBookByISBNAsync(string isbn)
    {
        return await _dbSet
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.ISBN == isbn || b.ISBN13 == isbn);
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(b => 
                b.Title.ToLower().Contains(lowerSearchTerm) ||
                b.Subtitle!.ToLower().Contains(lowerSearchTerm) ||
                b.Description!.ToLower().Contains(lowerSearchTerm) ||
                b.ISBN.Contains(lowerSearchTerm) ||
                b.BookAuthors.Any(ba => 
                    ba.Author.FirstName.ToLower().Contains(lowerSearchTerm) ||
                    ba.Author.LastName.ToLower().Contains(lowerSearchTerm)) ||
                b.Publisher!.Name.ToLower().Contains(lowerSearchTerm))
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableBooksAsync()
    {
        return await _dbSet
            .Where(b => b.Status == BookStatus.Available && b.AvailableQuantity > 0)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .ToListAsync();
    }

    public async Task<bool> UpdateBookAvailabilityAsync(int bookId, int availableQuantity)
    {
        var book = await _dbSet.FindAsync(bookId);
        if (book == null) return false;

        book.AvailableQuantity = availableQuantity;
        book.UpdatedAt = DateTime.UtcNow;
        
        // Update status based on availability
        if (availableQuantity > 0)
        {
            book.Status = BookStatus.Available;
        }
        else if (book.Status == BookStatus.Available)
        {
            book.Status = BookStatus.CheckedOut;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Book?> GetBookWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.BookCategories).ThenInclude(bc => bc.Category)
            .Include(b => b.Publisher)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}