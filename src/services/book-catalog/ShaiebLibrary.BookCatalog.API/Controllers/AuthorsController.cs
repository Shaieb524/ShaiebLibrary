using Microsoft.AspNetCore.Mvc;
using ShaiebLibrary.BookCatalog.Application.DTOs.Author;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

namespace ShaiebLibrary.BookCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
    {
        _authorService = authorService;
        _logger = logger;
    }

    /// <summary>
    /// Get all authors with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAuthors([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _authorService.GetAuthorsAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving authors");
            return StatusCode(500, "An error occurred while retrieving authors");
        }
    }

    /// <summary>
    /// Get a specific author by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(int id)
    {
        try
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
                return NotFound($"Author with ID {id} not found");

            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving author {AuthorId}", id);
            return StatusCode(500, "An error occurred while retrieving the author");
        }
    }

    /// <summary>
    /// Get author with their books
    /// </summary>
    [HttpGet("{id}/books")]
    public async Task<IActionResult> GetAuthorWithBooks(int id)
    {
        try
        {
            var author = await _authorService.GetAuthorWithBooksAsync(id);
            if (author == null)
                return NotFound($"Author with ID {id} not found");

            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving author with books {AuthorId}", id);
            return StatusCode(500, "An error occurred while retrieving the author");
        }
    }

    /// <summary>
    /// Search authors by name
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchAuthors([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var authors = await _authorService.SearchAuthorsByNameAsync(searchTerm);
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching authors");
            return StatusCode(500, "An error occurred while searching authors");
        }
    }

    /// <summary>
    /// Get authors by book
    /// </summary>
    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetAuthorsByBook(int bookId)
    {
        try
        {
            var authors = await _authorService.GetAuthorsByBookAsync(bookId);
            return Ok(authors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving authors by book {BookId}", bookId);
            return StatusCode(500, "An error occurred while retrieving authors");
        }
    }

    /// <summary>
    /// Create a new author
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto createAuthorDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var author = await _authorService.CreateAuthorAsync(createAuthorDto);
            return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating author");
            return StatusCode(500, "An error occurred while creating the author");
        }
    }

    /// <summary>
    /// Update an existing author
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(int id, [FromBody] CreateAuthorDto updateAuthorDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var author = await _authorService.UpdateAuthorAsync(id, updateAuthorDto);
            if (author == null)
                return NotFound($"Author with ID {id} not found");

            return Ok(author);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating author {AuthorId}", id);
            return StatusCode(500, "An error occurred while updating the author");
        }
    }

    /// <summary>
    /// Delete an author
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(int id)
    {
        try
        {
            var result = await _authorService.DeleteAuthorAsync(id);
            if (!result)
                return NotFound($"Author with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting author {AuthorId}", id);
            return StatusCode(500, "An error occurred while deleting the author");
        }
    }
}