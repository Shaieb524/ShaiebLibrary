using AutoMapper;
using FluentAssertions;
using Moq;
using ShaiebLibrary.BookCatalog.Application.DTOs.Category;
using ShaiebLibrary.BookCatalog.Application.Mapping;
using ShaiebLibrary.BookCatalog.Application.Services.Implementations;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly IMapper _mapper;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CategoryMappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _categoryService = new CategoryService(_mockCategoryRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WhenCategoryExists_ReturnsCategoryDto()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateSampleCategory(categoryId);
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(categoryId);
        result.Name.Should().Be("Fiction");
        result.Description.Should().Be("Fiction books category");
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var categoryId = 999;
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidDto_ReturnsCreatedCategory()
    {
        // Arrange
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Science Fiction",
            Description = "Science fiction books",
            ParentCategoryId = null
        };

        var createdCategory = new Category
        {
            Id = 1,
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            ParentCategoryId = createCategoryDto.ParentCategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _categoryService.CreateCategoryAsync(createCategoryDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createCategoryDto.Name);
        result.Description.Should().Be(createCategoryDto.Description);
        result.ParentCategoryId.Should().Be(createCategoryDto.ParentCategoryId);

        _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidParentCategoryId_ReturnsCreatedCategory()
    {
        // Arrange
        var parentCategoryId = 1;
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Mystery",
            Description = "Mystery books",
            ParentCategoryId = parentCategoryId
        };

        var createdCategory = new Category
        {
            Id = 2,
            Name = createCategoryDto.Name,
            Description = createCategoryDto.Description,
            ParentCategoryId = createCategoryDto.ParentCategoryId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockCategoryRepository.Setup(r => r.ExistsAsync(parentCategoryId))
            .ReturnsAsync(true);
        _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync(createdCategory);

        // Act
        var result = await _categoryService.CreateCategoryAsync(createCategoryDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createCategoryDto.Name);
        result.ParentCategoryId.Should().Be(parentCategoryId);

        _mockCategoryRepository.Verify(r => r.ExistsAsync(parentCategoryId), Times.Once);
        _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithInvalidParentCategoryId_ThrowsInvalidOperationException()
    {
        // Arrange
        var parentCategoryId = 999;
        var createCategoryDto = new CreateCategoryDto
        {
            Name = "Mystery",
            Description = "Mystery books",
            ParentCategoryId = parentCategoryId
        };

        _mockCategoryRepository.Setup(r => r.ExistsAsync(parentCategoryId))
            .ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _categoryService.CreateCategoryAsync(createCategoryDto));

        exception.Message.Should().Contain($"Parent category with ID {parentCategoryId} does not exist");
        _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithValidData_ReturnsUpdatedCategory()
    {
        // Arrange
        var categoryId = 1;
        var existingCategory = CreateSampleCategory(categoryId);
        var updateCategoryDto = new CreateCategoryDto
        {
            Name = "Updated Fiction",
            Description = "Updated description",
            ParentCategoryId = null
        };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(existingCategory);

        // Act
        var result = await _categoryService.UpdateCategoryAsync(categoryId, updateCategoryDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(updateCategoryDto.Name);
        result.Description.Should().Be(updateCategoryDto.Description);

        _mockCategoryRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var categoryId = 999;
        var updateCategoryDto = new CreateCategoryDto
        {
            Name = "Updated Fiction",
            Description = "Updated description"
        };

        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.UpdateCategoryAsync(categoryId, updateCategoryDto);

        // Assert
        result.Should().BeNull();
        _mockCategoryRepository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task GetRootCategoriesAsync_ReturnsRootCategories()
    {
        // Arrange
        var rootCategories = new List<Category>
        {
            CreateSampleCategory(1),
            CreateSampleCategory(2, "Non-Fiction")
        };

        _mockCategoryRepository.Setup(r => r.GetRootCategoriesAsync())
            .ReturnsAsync(rootCategories);

        // Act
        var result = await _categoryService.GetRootCategoriesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Fiction");
        result.Should().Contain(c => c.Name == "Non-Fiction");
    }

    [Fact]
    public async Task GetSubCategoriesAsync_ReturnsSubCategories()
    {
        // Arrange
        var parentCategoryId = 1;
        var subCategories = new List<Category>
        {
            CreateSampleCategory(2, "Mystery", parentCategoryId),
            CreateSampleCategory(3, "Romance", parentCategoryId)
        };

        _mockCategoryRepository.Setup(r => r.GetSubCategoriesAsync(parentCategoryId))
            .ReturnsAsync(subCategories);

        // Act
        var result = await _categoryService.GetSubCategoriesAsync(parentCategoryId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Mystery");
        result.Should().Contain(c => c.Name == "Romance");
    }

    [Fact]
    public async Task SearchCategoriesByNameAsync_ReturnsMatchingCategories()
    {
        // Arrange
        var searchTerm = "Fiction";
        var categories = new List<Category>
        {
            CreateSampleCategory(1, "Fiction"),
            CreateSampleCategory(2, "Science Fiction")
        };

        _mockCategoryRepository.Setup(r => r.SearchCategoriesByNameAsync(searchTerm))
            .ReturnsAsync(categories);

        // Act
        var result = await _categoryService.SearchCategoriesByNameAsync(searchTerm);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Name == "Fiction");
        result.Should().Contain(c => c.Name == "Science Fiction");
    }

    [Fact]
    public async Task CategoryExistsAsync_WhenCategoryExists_ReturnsTrue()
    {
        // Arrange
        var categoryId = 1;
        _mockCategoryRepository.Setup(r => r.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.CategoryExistsAsync(categoryId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CategoryExistsAsync_WhenCategoryDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var categoryId = 999;
        _mockCategoryRepository.Setup(r => r.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        // Act
        var result = await _categoryService.CategoryExistsAsync(categoryId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CanDeleteCategoryAsync_WhenCategoryHasNoSubCategoriesOrBooks_ReturnsTrue()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateSampleCategory(categoryId);
        category.SubCategories = new List<Category>();
        
        var categoryWithBooks = CreateSampleCategory(categoryId);
        categoryWithBooks.BookCategories = new List<BookCategory>();

        _mockCategoryRepository.Setup(r => r.GetCategoryWithSubCategoriesAsync(categoryId))
            .ReturnsAsync(category);
        _mockCategoryRepository.Setup(r => r.GetCategoryWithBooksAsync(categoryId))
            .ReturnsAsync(categoryWithBooks);

        // Act
        var result = await _categoryService.CanDeleteCategoryAsync(categoryId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task CanDeleteCategoryAsync_WhenCategoryHasSubCategories_ReturnsFalse()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateSampleCategory(categoryId);
        category.SubCategories = new List<Category> { CreateSampleCategory(2) };

        _mockCategoryRepository.Setup(r => r.GetCategoryWithSubCategoriesAsync(categoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.CanDeleteCategoryAsync(categoryId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenCanDelete_ReturnsTrue()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateSampleCategory(categoryId);
        category.SubCategories = new List<Category>();
        
        var categoryWithBooks = CreateSampleCategory(categoryId);
        categoryWithBooks.BookCategories = new List<BookCategory>();

        _mockCategoryRepository.Setup(r => r.GetCategoryWithSubCategoriesAsync(categoryId))
            .ReturnsAsync(category);
        _mockCategoryRepository.Setup(r => r.GetCategoryWithBooksAsync(categoryId))
            .ReturnsAsync(categoryWithBooks);
        _mockCategoryRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        result.Should().BeTrue();
        _mockCategoryRepository.Verify(r => r.DeleteAsync(category), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WhenCategoryHasSubCategories_ThrowsInvalidOperationException()
    {
        // Arrange
        var categoryId = 1;
        var category = CreateSampleCategory(categoryId);
        category.SubCategories = new List<Category> { CreateSampleCategory(2) };

        _mockCategoryRepository.Setup(r => r.GetCategoryWithSubCategoriesAsync(categoryId))
            .ReturnsAsync(category);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _categoryService.DeleteCategoryAsync(categoryId));

        exception.Message.Should().Contain("Cannot delete category that has subcategories or associated books");
        _mockCategoryRepository.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never);
    }

    // Helper methods
    private static Category CreateSampleCategory(int id, string name = "Fiction", int? parentId = null)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Description = $"{name} books category",
            ParentCategoryId = parentId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            SubCategories = new List<Category>(),
            BookCategories = new List<BookCategory>()
        };
    }
}