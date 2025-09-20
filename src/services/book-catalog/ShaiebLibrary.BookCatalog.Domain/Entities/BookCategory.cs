namespace ShaiebLibrary.BookCatalog.Domain.Entities;

public class BookCategory
{
    public int BookId { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public Book Book { get; set; } = null!;
    public Category Category { get; set; } = null!;
}