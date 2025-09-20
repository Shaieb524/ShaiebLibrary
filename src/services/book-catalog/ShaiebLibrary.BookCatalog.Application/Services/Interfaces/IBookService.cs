using ShaiebLibrary.BookCatalog.Application.DTOs.Book;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;
using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

public interface IBookService
{
    // Basic CRUD operations
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<BookDto?> GetBookWithDetailsAsync(int id);
    Task<PagedResult<BookDto>> GetBooksAsync(int page = 1, int pageSize = 10);
    Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
    Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto updateBookDto);
    Task<bool> DeleteBookAsync(int id);

    // Search and filtering
    Task<PagedResult<BookDto>> SearchBooksAsync(BookSearchCriteria criteria);
    Task<IEnumerable<BookDto>> GetBooksByLanguageAsync(Language language);
    Task<IEnumerable<BookDto>> GetBooksByStatusAsync(BookStatus status);
    Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId);
    Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId);
    Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId);

    // Special operations
    Task<BookDto?> GetBookByISBNAsync(string isbn);
    Task<IEnumerable<BookDto>> GetAvailableBooksAsync();
    Task<bool> UpdateBookAvailabilityAsync(int bookId, int availableQuantity);
    Task<bool> IsBookAvailableAsync(int bookId);

    // Validation
    Task<bool> IsISBNUniqueAsync(string isbn, int? excludeBookId = null);
}