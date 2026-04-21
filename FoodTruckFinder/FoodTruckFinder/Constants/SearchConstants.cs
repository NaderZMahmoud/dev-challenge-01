namespace FoodTruckFinder.Constants;

/// <summary>
/// Centralized constants for food truck search operations.
/// </summary>
public static class SearchConstants
{
    /// <summary>
    /// Default number of results to return if not specified.
    /// </summary>
    public const int DefaultSearchLimit = 5;

    /// <summary>
    /// Maximum number of results to return in a single search.
    /// </summary>
    public const int MaxSearchLimit = 100;

    /// <summary>
    /// Minimum search limit (at least 1 result).
    /// </summary>
    public const int MinSearchLimit = 1;

    /// <summary>
    /// Earth radius in miles, used for distance calculations (Haversine formula).
    /// </summary>
    public const double EarthRadiusMiles = 3959;

    /// <summary>
    /// Status filter for approved food trucks.
    /// </summary>
    public const string ApprovedStatus = "APPROVED";

    /// <summary>
    /// Minimum valid latitude (-90 degrees).
    /// </summary>
    public const double MinLatitude = -90;

    /// <summary>
    /// Maximum valid latitude (90 degrees).
    /// </summary>
    public const double MaxLatitude = 90;

    /// <summary>
    /// Minimum valid longitude (-180 degrees).
    /// </summary>
    public const double MinLongitude = -180;

    /// <summary>
    /// Maximum valid longitude (180 degrees).
    /// </summary>
    public const double MaxLongitude = 180;
}
