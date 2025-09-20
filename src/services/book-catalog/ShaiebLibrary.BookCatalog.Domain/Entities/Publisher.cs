namespace ShaiebLibrary.BookCatalog.Domain.Entities;

public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Website { get; set; }
    public DateTime? EstablishedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    public ICollection<Book> Books { get; set; } = new List<Book>();
}