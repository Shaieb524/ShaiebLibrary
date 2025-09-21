using AutoMapper;
using FluentAssertions;
using Moq;
using ShaiebLibrary.BookCatalog.Application.DTOs.Publisher;
using ShaiebLibrary.BookCatalog.Application.Mapping;
using ShaiebLibrary.BookCatalog.Application.Services.Implementations;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Tests.Services;

public class PublisherServiceTests
{
    private readonly Mock<IPublisherRepository> _mockPublisherRepository;
    private readonly IMapper _mapper;
    private readonly PublisherService _publisherService;

    public PublisherServiceTests()
    {
        _mockPublisherRepository = new Mock<IPublisherRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PublisherMappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _publisherService = new PublisherService(_mockPublisherRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetPublisherByIdAsync_WhenPublisherExists_ReturnsPublisherDto()
    {
        // Arrange
        var publisherId = 1;
        var publisher = CreateSamplePublisher(publisherId);
        _mockPublisherRepository.Setup(r => r.GetByIdAsync(publisherId))
            .ReturnsAsync(publisher);

        // Act
        var result = await _publisherService.GetPublisherByIdAsync(publisherId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(publisherId);
        result.Name.Should().Be("Penguin Random House");
        result.Address.Should().Be("123 Publisher St");
        result.Website.Should().Be("https://www.penguinrandomhouse.com");
    }

    [Fact]
    public async Task GetPublisherByIdAsync_WhenPublisherDoesNotExist_ReturnsNull()
    {
        // Arrange
        var publisherId = 999;
        _mockPublisherRepository.Setup(r => r.GetByIdAsync(publisherId))
            .ReturnsAsync((Publisher?)null);

        // Act
        var result = await _publisherService.GetPublisherByIdAsync(publisherId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreatePublisherAsync_WithValidDto_ReturnsCreatedPublisher()
    {
        // Arrange
        var createPublisherDto = new CreatePublisherDto
        {
            Name = "Harper Collins",
            Address = "456 Publishing Ave",
            Website = "https://www.harpercollins.com",
            EstablishedDate = new DateTime(1989, 1, 1)
        };

        var createdPublisher = new Publisher
        {
            Id = 1,
            Name = createPublisherDto.Name,
            Address = createPublisherDto.Address,
            Website = createPublisherDto.Website,
            EstablishedDate = createPublisherDto.EstablishedDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(createPublisherDto.Name))
            .ReturnsAsync((Publisher?)null);
        _mockPublisherRepository.Setup(r => r.AddAsync(It.IsAny<Publisher>()))
            .ReturnsAsync(createdPublisher);

        // Act
        var result = await _publisherService.CreatePublisherAsync(createPublisherDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createPublisherDto.Name);
        result.Address.Should().Be(createPublisherDto.Address);
        result.Website.Should().Be(createPublisherDto.Website);
        result.EstablishedDate.Should().Be(createPublisherDto.EstablishedDate);

        _mockPublisherRepository.Verify(r => r.AddAsync(It.IsAny<Publisher>()), Times.Once);
    }

    [Fact]
    public async Task CreatePublisherAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var createPublisherDto = new CreatePublisherDto
        {
            Name = "Penguin Random House",
            Address = "456 Publishing Ave"
        };

        var existingPublisher = CreateSamplePublisher(1);
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(createPublisherDto.Name))
            .ReturnsAsync(existingPublisher);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _publisherService.CreatePublisherAsync(createPublisherDto));

        exception.Message.Should().Contain($"Publisher with name '{createPublisherDto.Name}' already exists");
        _mockPublisherRepository.Verify(r => r.AddAsync(It.IsAny<Publisher>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePublisherAsync_WithValidData_ReturnsUpdatedPublisher()
    {
        // Arrange
        var publisherId = 1;
        var existingPublisher = CreateSamplePublisher(publisherId);
        var updatePublisherDto = new CreatePublisherDto
        {
            Name = "Updated Publisher Name",
            Address = "Updated Address",
            Website = "https://www.updated.com",
            EstablishedDate = new DateTime(1990, 1, 1)
        };

        _mockPublisherRepository.Setup(r => r.GetByIdAsync(publisherId))
            .ReturnsAsync(existingPublisher);
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(updatePublisherDto.Name))
            .ReturnsAsync((Publisher?)null);

        // Act
        var result = await _publisherService.UpdatePublisherAsync(publisherId, updatePublisherDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(updatePublisherDto.Name);
        result.Address.Should().Be(updatePublisherDto.Address);
        result.Website.Should().Be(updatePublisherDto.Website);
        result.EstablishedDate.Should().Be(updatePublisherDto.EstablishedDate);

        _mockPublisherRepository.Verify(r => r.UpdateAsync(It.IsAny<Publisher>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePublisherAsync_WhenPublisherDoesNotExist_ReturnsNull()
    {
        // Arrange
        var publisherId = 999;
        var updatePublisherDto = new CreatePublisherDto
        {
            Name = "Updated Publisher"
        };

        _mockPublisherRepository.Setup(r => r.GetByIdAsync(publisherId))
            .ReturnsAsync((Publisher?)null);

        // Act
        var result = await _publisherService.UpdatePublisherAsync(publisherId, updatePublisherDto);

        // Assert
        result.Should().BeNull();
        _mockPublisherRepository.Verify(r => r.UpdateAsync(It.IsAny<Publisher>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePublisherAsync_WithDuplicateName_ThrowsInvalidOperationException()
    {
        // Arrange
        var publisherId = 1;
        var existingPublisher = CreateSamplePublisher(publisherId);
        var otherPublisher = CreateSamplePublisher(2, "Other Publisher");
        var updatePublisherDto = new CreatePublisherDto
        {
            Name = "Other Publisher"
        };

        _mockPublisherRepository.Setup(r => r.GetByIdAsync(publisherId))
            .ReturnsAsync(existingPublisher);
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(updatePublisherDto.Name))
            .ReturnsAsync(otherPublisher);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _publisherService.UpdatePublisherAsync(publisherId, updatePublisherDto));

        exception.Message.Should().Contain($"Publisher with name '{updatePublisherDto.Name}' already exists");
        _mockPublisherRepository.Verify(r => r.UpdateAsync(It.IsAny<Publisher>()), Times.Never);
    }

    [Fact]
    public async Task DeletePublisherAsync_WhenPublisherHasNoBooks_ReturnsTrue()
    {
        // Arrange
        var publisherId = 1;
        var publisher = CreateSamplePublisher(publisherId);
        publisher.Books = new List<Book>();

        _mockPublisherRepository.Setup(r => r.GetPublisherWithBooksAsync(publisherId))
            .ReturnsAsync(publisher);

        // Act
        var result = await _publisherService.DeletePublisherAsync(publisherId);

        // Assert
        result.Should().BeTrue();
        _mockPublisherRepository.Verify(r => r.DeleteAsync(publisher), Times.Once);
    }

    [Fact]
    public async Task DeletePublisherAsync_WhenPublisherHasBooks_ThrowsInvalidOperationException()
    {
        // Arrange
        var publisherId = 1;
        var publisher = CreateSamplePublisher(publisherId);
        publisher.Books = new List<Book> { new Book { Id = 1, Title = "Test Book" } };

        _mockPublisherRepository.Setup(r => r.GetPublisherWithBooksAsync(publisherId))
            .ReturnsAsync(publisher);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _publisherService.DeletePublisherAsync(publisherId));

        exception.Message.Should().Contain("Cannot delete publisher that has associated books");
        _mockPublisherRepository.Verify(r => r.DeleteAsync(It.IsAny<Publisher>()), Times.Never);
    }

    [Fact]
    public async Task DeletePublisherAsync_WhenPublisherDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var publisherId = 999;
        _mockPublisherRepository.Setup(r => r.GetPublisherWithBooksAsync(publisherId))
            .ReturnsAsync((Publisher?)null);

        // Act
        var result = await _publisherService.DeletePublisherAsync(publisherId);

        // Assert
        result.Should().BeFalse();
        _mockPublisherRepository.Verify(r => r.DeleteAsync(It.IsAny<Publisher>()), Times.Never);
    }

    [Fact]
    public async Task GetPublisherByNameAsync_WhenPublisherExists_ReturnsPublisherDto()
    {
        // Arrange
        var publisherName = "Penguin Random House";
        var publisher = CreateSamplePublisher(1);
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(publisherName))
            .ReturnsAsync(publisher);

        // Act
        var result = await _publisherService.GetPublisherByNameAsync(publisherName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(publisherName);
    }

    [Fact]
    public async Task GetPublisherByNameAsync_WhenPublisherDoesNotExist_ReturnsNull()
    {
        // Arrange
        var publisherName = "Non-existent Publisher";
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(publisherName))
            .ReturnsAsync((Publisher?)null);

        // Act
        var result = await _publisherService.GetPublisherByNameAsync(publisherName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchPublishersByNameAsync_ReturnsMatchingPublishers()
    {
        // Arrange
        var searchTerm = "Penguin";
        var publishers = new List<Publisher>
        {
            CreateSamplePublisher(1, "Penguin Random House"),
            CreateSamplePublisher(2, "Penguin Classics")
        };

        _mockPublisherRepository.Setup(r => r.SearchPublishersByNameAsync(searchTerm))
            .ReturnsAsync(publishers);

        // Act
        var result = await _publisherService.SearchPublishersByNameAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Penguin Random House");
        result.Should().Contain(p => p.Name == "Penguin Classics");
    }

    [Fact]
    public async Task PublisherExistsAsync_WhenPublisherExists_ReturnsTrue()
    {
        // Arrange
        var publisherId = 1;
        _mockPublisherRepository.Setup(r => r.ExistsAsync(publisherId))
            .ReturnsAsync(true);

        // Act
        var result = await _publisherService.PublisherExistsAsync(publisherId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PublisherExistsAsync_WhenPublisherDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var publisherId = 999;
        _mockPublisherRepository.Setup(r => r.ExistsAsync(publisherId))
            .ReturnsAsync(false);

        // Act
        var result = await _publisherService.PublisherExistsAsync(publisherId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsPublisherNameUniqueAsync_WithUniqueNameAndNoExclusion_ReturnsTrue()
    {
        // Arrange
        var publisherName = "Unique Publisher";
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(publisherName))
            .ReturnsAsync((Publisher?)null);

        // Act
        var result = await _publisherService.IsPublisherNameUniqueAsync(publisherName);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPublisherNameUniqueAsync_WithExistingNameAndSameIdExclusion_ReturnsTrue()
    {
        // Arrange
        var publisherName = "Existing Publisher";
        var publisherId = 1;
        var existingPublisher = CreateSamplePublisher(publisherId);
        
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(publisherName))
            .ReturnsAsync(existingPublisher);

        // Act
        var result = await _publisherService.IsPublisherNameUniqueAsync(publisherName, publisherId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsPublisherNameUniqueAsync_WithExistingNameAndDifferentIdExclusion_ReturnsFalse()
    {
        // Arrange
        var publisherName = "Existing Publisher";
        var publisherId = 1;
        var differentPublisherId = 2;
        var existingPublisher = CreateSamplePublisher(publisherId);
        
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(publisherName))
            .ReturnsAsync(existingPublisher);

        // Act
        var result = await _publisherService.IsPublisherNameUniqueAsync(publisherName, differentPublisherId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetOrCreatePublisherAsync_WhenPublisherExists_ReturnsExistingPublisher()
    {
        // Arrange
        var publisherName = "Existing Publisher";
        var existingPublisher = CreateSamplePublisher(1, publisherName);
        
        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(publisherName))
            .ReturnsAsync(existingPublisher);

        // Act
        var result = await _publisherService.GetOrCreatePublisherAsync(publisherName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(publisherName);
        result.Id.Should().Be(1);

        _mockPublisherRepository.Verify(r => r.AddAsync(It.IsAny<Publisher>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreatePublisherAsync_WhenPublisherDoesNotExist_CreatesNewPublisher()
    {
        // Arrange
        var publisherName = "New Publisher";
        var createdPublisher = new Publisher
        {
            Id = 1,
            Name = publisherName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockPublisherRepository.Setup(r => r.GetPublisherByNameAsync(publisherName))
            .ReturnsAsync((Publisher?)null);
        _mockPublisherRepository.Setup(r => r.AddAsync(It.IsAny<Publisher>()))
            .ReturnsAsync(createdPublisher);

        // Act
        var result = await _publisherService.GetOrCreatePublisherAsync(publisherName);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(publisherName);

        _mockPublisherRepository.Verify(r => r.AddAsync(It.IsAny<Publisher>()), Times.Once);
    }

    // Helper methods
    private static Publisher CreateSamplePublisher(int id, string name = "Penguin Random House")
    {
        return new Publisher
        {
            Id = id,
            Name = name,
            Address = "123 Publisher St",
            Website = "https://www.penguinrandomhouse.com",
            EstablishedDate = new DateTime(1927, 7, 30),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Books = new List<Book>()
        };
    }
}