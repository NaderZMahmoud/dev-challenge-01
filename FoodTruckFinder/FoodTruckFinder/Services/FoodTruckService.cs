using FoodTruckFinder.Constants;
using FoodTruckFinder.Data;
using FoodTruckFinder.Models;
using FoodTruckFinder.Services.Models;

namespace FoodTruckFinder.Services;

public interface IFoodTruckService
{
    Task<SearchResponse> SearchFoodTrucksAsync(FoodTruckSearchQuery query);
    Task<int> GetTotalFoodTrucksCountAsync();
}

public class FoodTruckService : IFoodTruckService
{
    private readonly IFoodTruckRepository _repository;
    private readonly ILogger<FoodTruckService> _logger;

    public FoodTruckService(ILogger<FoodTruckService> logger, IFoodTruckRepository repository)
    {
        _logger = logger;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<int> GetTotalFoodTrucksCountAsync()
    {
        return await _repository.GetCountAsync();
    }

    public async Task<SearchResponse> SearchFoodTrucksAsync(FoodTruckSearchQuery query)
    {
        // Get all trucks from the repository
        var allTrucks = await _repository.GetAllAsync();

        var filteredTrucks = allTrucks.AsEnumerable();

        // Filter by preferred food if specified
        if (!string.IsNullOrWhiteSpace(query.PreferredFood))
        {
            filteredTrucks = filteredTrucks.Where(ft =>
                !string.IsNullOrWhiteSpace(ft.FoodItems) &&
                ft.FoodItems.Contains(query.PreferredFood, StringComparison.OrdinalIgnoreCase));
        }

        // Filter only approved trucks
        filteredTrucks = filteredTrucks.Where(ft => 
                ft.Status.Equals(SearchConstants.ApprovedStatus, StringComparison.OrdinalIgnoreCase));

        // Calculate distances and sort
        var trucksWithDistance = filteredTrucks
            .Select(ft => new FoodTruckSearchResult(
                ft,
                CalculateDistance(
                    query.Latitude, query.Longitude,
                    ft.Latitude, ft.Longitude)))
            .OrderBy(result => result.Distance)
            .Take(query.Limit)
            .ToList();

        _logger.LogInformation(
            "Search completed: Found {Count} food trucks for food='{Food}' near ({Lat}, {Lon})",
            trucksWithDistance.Count,
            query.PreferredFood ?? "any",
            query.Latitude,
            query.Longitude);

        var response = new SearchResponse
        {
            SearchLatitude = query.Latitude,
            SearchLongitude = query.Longitude,
            PreferredFood = query.PreferredFood,
            TotalResults = trucksWithDistance.Count,
            Results = [.. trucksWithDistance.Select(result => new FoodTruckResult
            {
                Name = result.Truck.Applicant,
                FacilityType = result.Truck.FacilityType,
                Address = result.Truck.Address,
                LocationDescription = result.Truck.LocationDescription,
                FoodItems = result.Truck.FoodItems,
                Status = result.Truck.Status,
                Latitude = result.Truck.Latitude,
                Longitude = result.Truck.Longitude,
                DistanceInMiles = Math.Round(result.Distance, 2),
                Schedule = result.Truck.Schedule
            })]
        };

        return response;
    }

    /// <summary>
    /// Calculate distance between two points using Haversine formula
    /// Returns distance in miles
    /// </summary>
    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return SearchConstants.EarthRadiusMiles * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}
