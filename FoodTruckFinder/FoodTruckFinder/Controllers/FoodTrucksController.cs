using Microsoft.AspNetCore.Mvc;
using FoodTruckFinder.Models;
using FoodTruckFinder.Services;
using FoodTruckFinder.Services.Models;

namespace FoodTruckFinder.Controllers;

/// <summary>
/// API endpoints for searching food trucks
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FoodTrucksController : ControllerBase
{
    private readonly IFoodTruckService _foodTruckService;
    private readonly ILogger<FoodTrucksController> _logger;

    public FoodTrucksController(
        IFoodTruckService foodTruckService, 
        ILogger<FoodTrucksController> logger)
    {
        _foodTruckService = foodTruckService;
        _logger = logger;
    }

    /// <summary>
    /// Search for food trucks near a location based on preferred food
    /// </summary>
    /// <param name="request">Search parameters including latitude, longitude, limit, and preferred food</param>
    /// <returns>List of food trucks ordered by distance</returns>
    /// <response code="200">Returns the list of food trucks</response>
    /// <response code="400">If the request is invalid</response>
    /// <response code="500">If an internal error occurs</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse>> Search([FromBody] SearchRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Map from API DTO to internal domain model
            var query = new FoodTruckSearchQuery(
                latitude: request.Latitude!.Value,
                longitude: request.Longitude!.Value,
                limit: request.Limit,
                preferredFood: request.PreferredFood);

            var result = await _foodTruckService.SearchFoodTrucksAsync(query);
            
            if (result.TotalResults == 0)
            {
                _logger.LogInformation(
                    "No food trucks found for search: Lat={Lat}, Lon={Lon}, Food={Food}",
                    request.Latitude, request.Longitude, request.PreferredFood ?? "any");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for food trucks");
            return StatusCode(
                StatusCodes.Status500InternalServerError, 
                new { error = "An error occurred while searching for food trucks" });
        }
    }

    /// <summary>
    /// Search for food trucks using query parameters (alternative endpoint)
    /// </summary>
    /// <param name="latitude">Latitude of search location</param>
    /// <param name="longitude">Longitude of search location</param>
    /// <param name="limit">Maximum number of results (1-100)</param>
    /// <param name="preferredFood">Optional food type to filter by</param>
    /// <returns>List of food trucks ordered by distance</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse>> SearchByQuery(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] int limit = 5,
        [FromQuery] string? preferredFood = null)
    {
        var request = new SearchRequest
        {
            Latitude = latitude,
            Longitude = longitude,
            Limit = limit,
            PreferredFood = preferredFood
        };

        return await Search(request);
    }

    /// <summary>
    /// Get API health status and statistics
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        var totalTrucks = _foodTruckService.GetTotalFoodTrucksCount();
        
        return Ok(new
        {
            status = "healthy",
            totalFoodTrucks = totalTrucks,
            timestamp = DateTime.UtcNow
        });
    }
}
