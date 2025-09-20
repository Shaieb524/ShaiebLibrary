using ShaiebLibrary.BookCatalog.Application.DTOs.Author;
using ShaiebLibrary.BookCatalog.Application.DTOs.Category;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;
using ShaiebLibrary.BookCatalog.Application.DTOs.Publisher;
using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Application.DTOs.Book;

public class BookDto : BaseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string? ISBN13 { get; set; }
    public Language Language { get; set; }
    public DateTime PublishedDate { get; set; }
    public int Pages { get; set; }
    public string? Description { get; set; }
    public BookStatus Status { get; set; }
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public decimal? Price { get; set; }
    public string? CoverImageUrl { get; set; }
    
    public PublisherDto? Publisher { get; set; }
    public IEnumerable<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
}