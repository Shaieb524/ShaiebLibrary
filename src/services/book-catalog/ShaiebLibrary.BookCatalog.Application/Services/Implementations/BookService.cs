using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Book;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;
using ShaiebLibrary.BookCatalog.Application.Extensions;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Enums;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Application.Services.Implementations;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPublisherRepository _publisherRepository;
    private readonly IMapper _mapper;

    public BookService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        ICategoryRepository categoryRepository,
        IPublisherRepository publisherRepository,
        IMapper mapper)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _categoryRepository = categoryRepository;
        _publisherRepository = publisherRepository;
        _mapper = mapper;
    }

    public async Task<BookDto?> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        return book == null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task<BookDto?> GetBookWithDetailsAsync(int id)
    {
        var book = await _bookRepository.GetBookWithDetailsAsync(id);
        return book == null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task<PagedResult<BookDto>> GetBooksAsync(int page = 1, int pageSize = 10)
    {
        var allBooks = await _bookRepository.GetAllAsync();
        var totalCount = allBooks.Count();
        var books = allBooks.Skip((page - 1) * pageSize).Take(pageSize);

        return _mapper.MapToPagedResult<Book, BookDto>(books, totalCount, page, pageSize);
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createBookDto)
    {
        // Validate ISBN uniqueness
        if (!await IsISBNUniqueAsync(createBookDto.ISBN))
        {
            throw new InvalidOperationException($"ISBN {createBookDto.ISBN} already exists.");
        }

        // Map DTO to entity
        var book = _mapper.Map<Book>(createBookDto);

        // Add the book first to get the ID
        var createdBook = await _bookRepository.AddAsync(book);

        // Handle many-to-many relationships
        await AddBookAuthorsAsync(createdBook.Id, createBookDto.AuthorIds);
        await AddBookCategoriesAsync(createdBook.Id, createBookDto.CategoryIds);

        // Get the book with all details for return
        var bookWithDetails = await _bookRepository.GetBookWithDetailsAsync(createdBook.Id);
        return _mapper.Map<BookDto>(bookWithDetails);
    }

    public async Task<BookDto?> UpdateBookAsync(int id, UpdateBookDto updateBookDto)
    {
        var existingBook = await _bookRepository.GetByIdAsync(id);
        if (existingBook == null)
            return null;

        // Validate ISBN uniqueness (excluding current book)
        if (!await IsISBNUniqueAsync(updateBookDto.ISBN, id))
        {
            throw new InvalidOperationException($"ISBN {updateBookDto.ISBN} already exists.");
        }

        // Map updates to existing entity
        _mapper.Map(updateBookDto, existingBook);
        await _bookRepository.UpdateAsync(existingBook);

        // Update many-to-many relationships
        await UpdateBookAuthorsAsync(id, updateBookDto.AuthorIds);
        await UpdateBookCategoriesAsync(id, updateBookDto.CategoryIds);

        // Get updated book with details
        var updatedBook = await _bookRepository.GetBookWithDetailsAsync(id);
        return _mapper.Map<BookDto>(updatedBook);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return false;

        await _bookRepository.DeleteAsync(book);
        return true;
    }

    public async Task<PagedResult<BookDto>> SearchBooksAsync(BookSearchCriteria criteria)
    {
        IEnumerable<Book> books;

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            books = await _bookRepository.SearchBooksAsync(criteria.SearchTerm);
        }
        else
        {
            books = await _bookRepository.GetAllAsync();
        }

        // Apply filters
        if (criteria.Language.HasValue)
            books = books.Where(b => b.Language == criteria.Language.Value);

        if (criteria.Status.HasValue)
            books = books.Where(b => b.Status == criteria.Status.Value);

        if (criteria.AuthorId.HasValue)
            books = books.Where(b => b.BookAuthors.Any(ba => ba.AuthorId == criteria.AuthorId.Value));

        if (criteria.CategoryId.HasValue)
            books = books.Where(b => b.BookCategories.Any(bc => bc.CategoryId == criteria.CategoryId.Value));

        if (criteria.PublisherId.HasValue)
            books = books.Where(b => b.PublisherId == criteria.PublisherId.Value);

        // Apply pagination
        var totalCount = books.Count();
        var pagedBooks = books.Skip((criteria.Page - 1) * criteria.PageSize).Take(criteria.PageSize);

        return _mapper.MapToPagedResult<Book, BookDto>(pagedBooks, totalCount, criteria.Page, criteria.PageSize);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByLanguageAsync(Language language)
    {
        var books = await _bookRepository.GetBooksByLanguageAsync(language);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByStatusAsync(BookStatus status)
    {
        var books = await _bookRepository.GetBooksByStatusAsync(status);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByAuthorAsync(int authorId)
    {
        var books = await _bookRepository.GetBooksByAuthorAsync(authorId);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByCategoryAsync(int categoryId)
    {
        var books = await _bookRepository.GetBooksByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<IEnumerable<BookDto>> GetBooksByPublisherAsync(int publisherId)
    {
        var books = await _bookRepository.GetBooksByPublisherAsync(publisherId);
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<BookDto?> GetBookByISBNAsync(string isbn)
    {
        var book = await _bookRepository.GetBookByISBNAsync(isbn);
        return book == null ? null : _mapper.Map<BookDto>(book);
    }

    public async Task<IEnumerable<BookDto>> GetAvailableBooksAsync()
    {
        var books = await _bookRepository.GetAvailableBooksAsync();
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }

    public async Task<bool> UpdateBookAvailabilityAsync(int bookId, int availableQuantity)
    {
        return await _bookRepository.UpdateBookAvailabilityAsync(bookId, availableQuantity);
    }

    public async Task<bool> IsBookAvailableAsync(int bookId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        return book != null && book.Status == BookStatus.Available && book.AvailableQuantity > 0;
    }

    public async Task<bool> IsISBNUniqueAsync(string isbn, int? excludeBookId = null)
    {
        var existingBook = await _bookRepository.GetBookByISBNAsync(isbn);
        
        if (existingBook == null)
            return true;

        return excludeBookId.HasValue && existingBook.Id == excludeBookId.Value;
    }

    // Private helper methods
    private async Task AddBookAuthorsAsync(int bookId, IEnumerable<int> authorIds)
    {
        foreach (var authorId in authorIds)
        {
            var author = await _authorRepository.GetByIdAsync(authorId);
            if (author != null)
            {
                // This would require a BookAuthor repository or direct context manipulation
                // For now, we'll assume the relationship is handled in the repository
            }
        }
    }

    private async Task AddBookCategoriesAsync(int bookId, IEnumerable<int> categoryIds)
    {
        foreach (var categoryId in categoryIds)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category != null)
            {
                // This would require a BookCategory repository or direct context manipulation
                // For now, we'll assume the relationship is handled in the repository
            }
        }
    }

    private async Task UpdateBookAuthorsAsync(int bookId, IEnumerable<int> authorIds)
    {
        // Implementation would remove existing relationships and add new ones
        // This requires direct context manipulation or additional repository methods
        await Task.CompletedTask;
    }

    private async Task UpdateBookCategoriesAsync(int bookId, IEnumerable<int> categoryIds)
    {
        // Implementation would remove existing relationships and add new ones
        // This requires direct context manipulation or additional repository methods
        await Task.CompletedTask;
    }
}