using ShaiebLibrary.BookCatalog.Application.DTOs.Author;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;

namespace ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

public interface IAuthorService
{
    // Basic CRUD operations
    Task<AuthorDto?> GetAuthorByIdAsync(int id);
    Task<AuthorDto?> GetAuthorWithBooksAsync(int id);
    Task<PagedResult<AuthorDto>> GetAuthorsAsync(int page = 1, int pageSize = 10);
    Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto);
    Task<AuthorDto?> UpdateAuthorAsync(int id, CreateAuthorDto updateAuthorDto);
    Task<bool> DeleteAuthorAsync(int id);

    // Search operations
    Task<IEnumerable<AuthorDto>> SearchAuthorsByNameAsync(string searchTerm);
    Task<AuthorDto?> GetAuthorByFullNameAsync(string firstName, string lastName);
    Task<IEnumerable<AuthorDto>> GetAuthorsByBookAsync(int bookId);

    // Validation
    Task<bool> AuthorExistsAsync(int id);
    Task<AuthorDto?> GetOrCreateAuthorAsync(string firstName, string lastName);
}