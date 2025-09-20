namespace ShaiebLibrary.BookCatalog.Application.DTOs.Common;

public abstract class BaseDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}