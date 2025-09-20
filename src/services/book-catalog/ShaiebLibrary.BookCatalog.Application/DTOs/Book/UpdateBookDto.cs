using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Application.DTOs.Book;

public class UpdateBookDto
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
    
    public int? PublisherId { get; set; }
    public IEnumerable<int> AuthorIds { get; set; } = new List<int>();
    public IEnumerable<int> CategoryIds { get; set; } = new List<int>();
}