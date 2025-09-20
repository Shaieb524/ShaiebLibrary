using Microsoft.AspNetCore.Mvc;
using ShaiebLibrary.BookCatalog.Application.DTOs.Book;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;
using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    /// <summary>
    /// Get all books with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _bookService.GetBooksAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving books");
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }

    /// <summary>
    /// Get a specific book by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(int id)
    {
        try
        {
            var book = await _bookService.GetBookWithDetailsAsync(id);
            if (book == null)
                return NotFound($"Book with ID {id} not found");

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving book {BookId}", id);
            return StatusCode(500, "An error occurred while retrieving the book");
        }
    }

    /// <summary>
    /// Get a book by ISBN
    /// </summary>
    [HttpGet("isbn/{isbn}")]
    public async Task<IActionResult> GetBookByISBN(string isbn)
    {
        try
        {
            var book = await _bookService.GetBookByISBNAsync(isbn);
            if (book == null)
                return NotFound($"Book with ISBN {isbn} not found");

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving book by ISBN {ISBN}", isbn);
            return StatusCode(500, "An error occurred while retrieving the book");
        }
    }

    /// <summary>
    /// Search books with various criteria
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchBooks([FromQuery] BookSearchCriteria criteria)
    {
        try
        {
            var result = await _bookService.SearchBooksAsync(criteria);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching books");
            return StatusCode(500, "An error occurred while searching books");
        }
    }

    /// <summary>
    /// Get books by language
    /// </summary>
    [HttpGet("language/{language}")]
    public async Task<IActionResult> GetBooksByLanguage(Language language)
    {
        try
        {
            var books = await _bookService.GetBooksByLanguageAsync(language);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving books by language {Language}", language);
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }

    /// <summary>
    /// Get books by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetBooksByStatus(BookStatus status)
    {
        try
        {
            var books = await _bookService.GetBooksByStatusAsync(status);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving books by status {Status}", status);
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }

    /// <summary>
    /// Get available books
    /// </summary>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableBooks()
    {
        try
        {
            var books = await _bookService.GetAvailableBooksAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving available books");
            return StatusCode(500, "An error occurred while retrieving available books");
        }
    }

    /// <summary>
    /// Get books by author
    /// </summary>
    [HttpGet("author/{authorId}")]
    public async Task<IActionResult> GetBooksByAuthor(int authorId)
    {
        try
        {
            var books = await _bookService.GetBooksByAuthorAsync(authorId);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving books by author {AuthorId}", authorId);
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }

    /// <summary>
    /// Get books by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetBooksByCategory(int categoryId)
    {
        try
        {
            var books = await _bookService.GetBooksByCategoryAsync(categoryId);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving books by category {CategoryId}", categoryId);
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookDto createBookDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _bookService.CreateBookAsync(createBookDto);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating book");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating book");
            return StatusCode(500, "An error occurred while creating the book");
        }
    }

    /// <summary>
    /// Update an existing book
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateBookDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _bookService.UpdateBookAsync(id, updateBookDto);
            if (book == null)
                return NotFound($"Book with ID {id} not found");

            return Ok(book);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating book {BookId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating book {BookId}", id);
            return StatusCode(500, "An error occurred while updating the book");
        }
    }

    /// <summary>
    /// Delete a book
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result)
                return NotFound($"Book with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting book {BookId}", id);
            return StatusCode(500, "An error occurred while deleting the book");
        }
    }

    /// <summary>
    /// Update book availability (for lending service)
    /// </summary>
    [HttpPut("{id}/availability")]
    public async Task<IActionResult> UpdateBookAvailability(int id, [FromBody] int availableQuantity)
    {
        try
        {
            var result = await _bookService.UpdateBookAvailabilityAsync(id, availableQuantity);
            if (!result)
                return NotFound($"Book with ID {id} not found");

            return Ok(new { BookId = id, AvailableQuantity = availableQuantity });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating book availability {BookId}", id);
            return StatusCode(500, "An error occurred while updating book availability");
        }
    }

    /// <summary>
    /// Check if book is available
    /// </summary>
    [HttpGet("{id}/availability")]
    public async Task<IActionResult> CheckBookAvailability(int id)
    {
        try
        {
            var isAvailable = await _bookService.IsBookAvailableAsync(id);
            return Ok(new { BookId = id, IsAvailable = isAvailable });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while checking book availability {BookId}", id);
            return StatusCode(500, "An error occurred while checking book availability");
        }
    }
}