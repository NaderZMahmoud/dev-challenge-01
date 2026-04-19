using System.ComponentModel.DataAnnotations;

namespace FoodTruckFinder.Models;

public class SearchRequest
{
    /// <summary>
    /// Latitude of the search location
    /// </summary>
    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude of the search location
    /// </summary>
    [Required]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public double Longitude { get; set; }

    /// <summary>
    /// Maximum number of results to return (1-100)
    /// </summary>
    [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
    public int Limit { get; set; } = 5;

    /// <summary>
    /// Food type to filter by (optional, case-insensitive)
    /// </summary>
    public string? PreferredFood { get; set; }
}
