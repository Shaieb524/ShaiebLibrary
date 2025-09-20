using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Domain.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<IEnumerable<Book>> GetBooksByLanguageAsync(Language language);
    Task<IEnumerable<Book>> GetBooksByStatusAsync(BookStatus status);
    Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId);
    Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
    Task<IEnumerable<Book>> GetBooksByPublisherAsync(int publisherId);
    Task<Book?> GetBookByISBNAsync(string isbn);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<Book>> GetAvailableBooksAsync();
    Task<bool> UpdateBookAvailabilityAsync(int bookId, int availableQuantity);
    Task<Book?> GetBookWithDetailsAsync(int id); // Include authors, categories, publisher
}