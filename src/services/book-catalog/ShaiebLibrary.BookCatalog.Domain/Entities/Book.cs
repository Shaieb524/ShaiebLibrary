using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Domain.Entities;

public class Book
{
    public int Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Foreign Keys
    public int? PublisherId { get; set; }
    
    // Navigation Properties
    public Publisher? Publisher { get; set; }
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}