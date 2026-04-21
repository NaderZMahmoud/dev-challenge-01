using System.ComponentModel.DataAnnotations;
using FoodTruckFinder.Constants;

namespace FoodTruckFinder.Models;

public class SearchRequest
{
    /// <summary>
    /// Latitude of the search location
    /// </summary>
    [Required]
    [Range(SearchConstants.MinLatitude, SearchConstants.MaxLatitude, ErrorMessage = "Latitude must be between -90 and 90")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Longitude of the search location
    /// </summary>
    [Required]
    [Range(SearchConstants.MinLongitude, SearchConstants.MaxLongitude, ErrorMessage = "Longitude must be between -180 and 180")]
    public double? Longitude { get; set; }

    /// <summary>
    /// Maximum number of results to return (1-100)
    /// </summary>
    [Range(SearchConstants.MinSearchLimit, SearchConstants.MaxSearchLimit, ErrorMessage = "Limit must be between 1 and 100")]
    public int Limit { get; set; } = SearchConstants.DefaultSearchLimit;

    /// <summary>
    /// Food type to filter by (optional, case-insensitive)
    /// </summary>
    public string? PreferredFood { get; set; }
}
