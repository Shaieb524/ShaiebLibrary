using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Domain.Entities;

public class BookAuthor
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }
    public AuthorRole Role { get; set; } = AuthorRole.Author;
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public Book Book { get; set; } = null!;
    public Author Author { get; set; } = null!;
}