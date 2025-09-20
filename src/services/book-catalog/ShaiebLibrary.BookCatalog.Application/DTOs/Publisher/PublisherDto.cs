using ShaiebLibrary.BookCatalog.Application.DTOs.Common;

namespace ShaiebLibrary.BookCatalog.Application.DTOs.Publisher;

public class PublisherDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Website { get; set; }
    public DateTime? EstablishedDate { get; set; }
}