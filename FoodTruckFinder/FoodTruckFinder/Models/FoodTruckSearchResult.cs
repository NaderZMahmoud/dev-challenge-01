namespace FoodTruckFinder.Models;

/// <summary>
/// Represents the result of a food truck search, combining the truck details with the calculated distance.
/// This immutable object makes it clear that distance is a search-result concern, not an intrinsic property of the truck.
/// </summary>
public record FoodTruckSearchResult(FoodTruck Truck, double Distance)
{
    /// <summary>
    /// The food truck details.
    /// </summary>
    public FoodTruck Truck { get; } = Truck ?? throw new ArgumentNullException(nameof(Truck));

    /// <summary>
    /// The calculated distance from the search location, in miles.
    /// </summary>
    public double Distance { get; } = Distance;
}
