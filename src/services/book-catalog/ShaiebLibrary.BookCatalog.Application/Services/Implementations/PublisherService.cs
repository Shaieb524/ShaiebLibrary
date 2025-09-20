using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;
using ShaiebLibrary.BookCatalog.Application.DTOs.Publisher;
using ShaiebLibrary.BookCatalog.Application.Extensions;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Application.Services.Implementations;

public class PublisherService : IPublisherService
{
    private readonly IPublisherRepository _publisherRepository;
    private readonly IMapper _mapper;

    public PublisherService(IPublisherRepository publisherRepository, IMapper mapper)
    {
        _publisherRepository = publisherRepository;
        _mapper = mapper;
    }

    public async Task<PublisherDto?> GetPublisherByIdAsync(int id)
    {
        var publisher = await _publisherRepository.GetByIdAsync(id);
        return publisher == null ? null : _mapper.Map<PublisherDto>(publisher);
    }

    public async Task<PublisherDto?> GetPublisherWithBooksAsync(int id)
    {
        var publisher = await _publisherRepository.GetPublisherWithBooksAsync(id);
        return publisher == null ? null : _mapper.Map<PublisherDto>(publisher);
    }

    public async Task<PagedResult<PublisherDto>> GetPublishersAsync(int page = 1, int pageSize = 10)
    {
        var allPublishers = await _publisherRepository.GetAllAsync();
        var totalCount = allPublishers.Count();
        var publishers = allPublishers.Skip((page - 1) * pageSize).Take(pageSize);

        return _mapper.MapToPagedResult<Publisher, PublisherDto>(publishers, totalCount, page, pageSize);
    }

    public async Task<PublisherDto> CreatePublisherAsync(CreatePublisherDto createPublisherDto)
    {
        // Validate name uniqueness
        if (!await IsPublisherNameUniqueAsync(createPublisherDto.Name))
        {
            throw new InvalidOperationException($"Publisher with name '{createPublisherDto.Name}' already exists.");
        }

        var publisher = _mapper.Map<Publisher>(createPublisherDto);
        var createdPublisher = await _publisherRepository.AddAsync(publisher);
        return _mapper.Map<PublisherDto>(createdPublisher);
    }

    public async Task<PublisherDto?> UpdatePublisherAsync(int id, CreatePublisherDto updatePublisherDto)
    {
        var existingPublisher = await _publisherRepository.GetByIdAsync(id);
        if (existingPublisher == null)
            return null;

        // Validate name uniqueness (excluding current publisher)
        if (!await IsPublisherNameUniqueAsync(updatePublisherDto.Name, id))
        {
            throw new InvalidOperationException($"Publisher with name '{updatePublisherDto.Name}' already exists.");
        }

        _mapper.Map(updatePublisherDto, existingPublisher);
        existingPublisher.UpdatedAt = DateTime.UtcNow;
        
        await _publisherRepository.UpdateAsync(existingPublisher);
        return _mapper.Map<PublisherDto>(existingPublisher);
    }

    public async Task<bool> DeletePublisherAsync(int id)
    {
        var publisher = await _publisherRepository.GetPublisherWithBooksAsync(id);
        if (publisher == null)
            return false;

        // Check if publisher has associated books
        if (publisher.Books.Any())
        {
            throw new InvalidOperationException("Cannot delete publisher that has associated books.");
        }

        await _publisherRepository.DeleteAsync(publisher);
        return true;
    }

    public async Task<IEnumerable<PublisherDto>> SearchPublishersByNameAsync(string searchTerm)
    {
        var publishers = await _publisherRepository.SearchPublishersByNameAsync(searchTerm);
        return _mapper.Map<IEnumerable<PublisherDto>>(publishers);
    }

    public async Task<PublisherDto?> GetPublisherByNameAsync(string name)
    {
        var publisher = await _publisherRepository.GetPublisherByNameAsync(name);
        return publisher == null ? null : _mapper.Map<PublisherDto>(publisher);
    }

    public async Task<bool> PublisherExistsAsync(int id)
    {
        return await _publisherRepository.ExistsAsync(id);
    }

    public async Task<bool> IsPublisherNameUniqueAsync(string name, int? excludePublisherId = null)
    {
        var existingPublisher = await _publisherRepository.GetPublisherByNameAsync(name);
        
        if (existingPublisher == null)
            return true;

        return excludePublisherId.HasValue && existingPublisher.Id == excludePublisherId.Value;
    }

    public async Task<PublisherDto?> GetOrCreatePublisherAsync(string name)
    {
        var existingPublisher = await _publisherRepository.GetPublisherByNameAsync(name);
        
        if (existingPublisher != null)
        {
            return _mapper.Map<PublisherDto>(existingPublisher);
        }

        var createPublisherDto = new CreatePublisherDto
        {
            Name = name
        };

        return await CreatePublisherAsync(createPublisherDto);
    }
}