using FoodTruckFinder.Models;

namespace FoodTruckFinder.Data;

/// <summary>
/// Repository interface for accessing food truck data.
/// This abstraction allows for different data sources (CSV, database, API, etc.)
/// without affecting the business logic layer.
/// </summary>
public interface IFoodTruckRepository
{
    /// <summary>
    /// Gets all approved food trucks from the data source.
    /// </summary>
    /// <returns>A collection of all food trucks.</returns>
    Task<IEnumerable<FoodTruck>> GetAllAsync();

    /// <summary>
    /// Gets all approved food trucks that offer the specified food type.
    /// </summary>
    /// <param name="foodType">The food type to filter by (case-insensitive substring match).</param>
    /// <returns>A collection of food trucks that offer the specified food type.</returns>
    Task<IEnumerable<FoodTruck>> GetByFoodTypeAsync(string foodType);

    /// <summary>
    /// Gets the total count of food trucks in the data source.
    /// </summary>
    /// <returns>The total number of food trucks available.</returns>
    Task<int> GetCountAsync();
}
