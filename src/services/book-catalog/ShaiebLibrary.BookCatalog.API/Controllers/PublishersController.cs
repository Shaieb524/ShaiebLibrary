using Microsoft.AspNetCore.Mvc;
using ShaiebLibrary.BookCatalog.Application.DTOs.Publisher;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

namespace ShaiebLibrary.BookCatalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublishersController : ControllerBase
{
    private readonly IPublisherService _publisherService;
    private readonly ILogger<PublishersController> _logger;

    public PublishersController(IPublisherService publisherService, ILogger<PublishersController> logger)
    {
        _publisherService = publisherService;
        _logger = logger;
    }

    /// <summary>
    /// Get all publishers with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPublishers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var result = await _publisherService.GetPublishersAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving publishers");
            return StatusCode(500, "An error occurred while retrieving publishers");
        }
    }

    /// <summary>
    /// Get a specific publisher by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPublisher(int id)
    {
        try
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null)
                return NotFound($"Publisher with ID {id} not found");

            return Ok(publisher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving publisher {PublisherId}", id);
            return StatusCode(500, "An error occurred while retrieving the publisher");
        }
    }

    /// <summary>
    /// Get publisher with their books
    /// </summary>
    [HttpGet("{id}/books")]
    public async Task<IActionResult> GetPublisherWithBooks(int id)
    {
        try
        {
            var publisher = await _publisherService.GetPublisherWithBooksAsync(id);
            if (publisher == null)
                return NotFound($"Publisher with ID {id} not found");

            return Ok(publisher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving publisher with books {PublisherId}", id);
            return StatusCode(500, "An error occurred while retrieving the publisher");
        }
    }

    /// <summary>
    /// Search publishers by name
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchPublishers([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term is required");

            var publishers = await _publisherService.SearchPublishersByNameAsync(searchTerm);
            return Ok(publishers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching publishers");
            return StatusCode(500, "An error occurred while searching publishers");
        }
    }

    /// <summary>
    /// Get publisher by name
    /// </summary>
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetPublisherByName(string name)
    {
        try
        {
            var publisher = await _publisherService.GetPublisherByNameAsync(name);
            if (publisher == null)
                return NotFound($"Publisher with name '{name}' not found");

            return Ok(publisher);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving publisher by name {Name}", name);
            return StatusCode(500, "An error occurred while retrieving the publisher");
        }
    }

    /// <summary>
    /// Create a new publisher
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePublisher([FromBody] CreatePublisherDto createPublisherDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var publisher = await _publisherService.CreatePublisherAsync(createPublisherDto);
            return CreatedAtAction(nameof(GetPublisher), new { id = publisher.Id }, publisher);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating publisher");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating publisher");
            return StatusCode(500, "An error occurred while creating the publisher");
        }
    }

    /// <summary>
    /// Update an existing publisher
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePublisher(int id, [FromBody] CreatePublisherDto updatePublisherDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var publisher = await _publisherService.UpdatePublisherAsync(id, updatePublisherDto);
            if (publisher == null)
                return NotFound($"Publisher with ID {id} not found");

            return Ok(publisher);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while updating publisher {PublisherId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating publisher {PublisherId}", id);
            return StatusCode(500, "An error occurred while updating the publisher");
        }
    }

    /// <summary>
    /// Delete a publisher
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePublisher(int id)
    {
        try
        {
            var result = await _publisherService.DeletePublisherAsync(id);
            if (!result)
                return NotFound($"Publisher with ID {id} not found");

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting publisher {PublisherId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting publisher {PublisherId}", id);
            return StatusCode(500, "An error occurred while deleting the publisher");
        }
    }
}