using FoodTruckFinder.Constants;

namespace FoodTruckFinder.Services.Models;

/// <summary>
/// Internal domain model for food truck search operations.
/// Used internally by the service layer to separate business logic from API contracts.
/// </summary>
public class FoodTruckSearchQuery
{
    /// <summary>
    /// Latitude of the search location (-90 to 90)
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude of the search location (-180 to 180)
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Maximum number of results to return (1-100)
    /// </summary>
    public int Limit { get; set; } = SearchConstants.DefaultSearchLimit;

    /// <summary>
    /// Food type to filter by (optional, case-insensitive)
    /// </summary>
    public string? PreferredFood { get; set; }

    public FoodTruckSearchQuery() { }

    public FoodTruckSearchQuery(double latitude, double longitude, int limit = SearchConstants.DefaultSearchLimit, string? preferredFood = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Limit = limit;
        PreferredFood = preferredFood;
    }
}
