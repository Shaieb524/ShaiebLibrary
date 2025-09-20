using ShaiebLibrary.BookCatalog.Domain.Entities;

namespace ShaiebLibrary.BookCatalog.Domain.Interfaces;

public interface IPublisherRepository : IRepository<Publisher>
{
    Task<Publisher?> GetPublisherByNameAsync(string name);
    Task<IEnumerable<Publisher>> SearchPublishersByNameAsync(string searchTerm);
    Task<Publisher?> GetPublisherWithBooksAsync(int id);
}