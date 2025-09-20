using ShaiebLibrary.BookCatalog.Application.DTOs.Common;
using ShaiebLibrary.BookCatalog.Application.DTOs.Publisher;

namespace ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

public interface IPublisherService
{
    // Basic CRUD operations
    Task<PublisherDto?> GetPublisherByIdAsync(int id);
    Task<PublisherDto?> GetPublisherWithBooksAsync(int id);
    Task<PagedResult<PublisherDto>> GetPublishersAsync(int page = 1, int pageSize = 10);
    Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createPublisherDto);
    Task<PublisherDto?> UpdatePublisherAsync(int id, CreatePublisherDto updatePublisherDto);
    Task<bool> DeletePublisherAsync(int id);

    // Search operations
    Task<IEnumerable<PublisherDto>> SearchPublishersByNameAsync(string searchTerm);
    Task<PublisherDto?> GetPublisherByNameAsync(string name);

    // Validation
    Task<bool> PublisherExistsAsync(int id);
    Task<bool> IsPublisherNameUniqueAsync(string name, int? excludePublisherId = null);
    Task<PublisherDto?> GetOrCreatePublisherAsync(string name);
}