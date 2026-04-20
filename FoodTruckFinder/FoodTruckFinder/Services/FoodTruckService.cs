using CsvHelper;
using CsvHelper.Configuration;
using FoodTruckFinder.Models;
using System.Globalization;

namespace FoodTruckFinder.Services;

public interface IFoodTruckService
{
    Task<SearchResponse> SearchFoodTrucksAsync(SearchRequest request);
    int GetTotalFoodTrucksCount();
}

public class FoodTruckService : IFoodTruckService
{
    private readonly List<FoodTruck> _foodTrucks;
    private readonly ILogger<FoodTruckService> _logger;

    public FoodTruckService(ILogger<FoodTruckService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _foodTrucks = LoadFoodTrucksFromCsv(configuration);
        _logger.LogInformation("Loaded {Count} food trucks from CSV", _foodTrucks.Count);
    }

    private List<FoodTruck> LoadFoodTrucksFromCsv(IConfiguration configuration)
    {
        var csvPath = configuration["DataSource:CsvPath"] ?? "Data/Mobile_Food_Facility_Permit.csv";
        
        if (!File.Exists(csvPath))
        {
            _logger.LogWarning("CSV file not found at {Path}. Returning empty dataset.", csvPath);
            return [];
        }

        try
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => args.Header.ToLower()
            });

            var records = csv.GetRecords<FoodTruckCsvRecord>().ToList();
            
            return [.. records
                .Select(r => new FoodTruck
                {
                    LocationId = r.Locationid ?? string.Empty,
                    Applicant = r.Applicant ?? string.Empty,
                    FacilityType = r.FacilityType ?? string.Empty,
                    LocationDescription = r.LocationDescription ?? string.Empty,
                    Address = r.Address ?? string.Empty,
                    Status = r.Status ?? string.Empty,
                    FoodItems = r.FoodItems ?? string.Empty,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Schedule = r.Schedule ?? string.Empty
                })
                .Where(ft => ft.Latitude != 0 && ft.Longitude != 0)];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading food trucks from CSV at {Path}", csvPath);
            return [];
        }
    }

    public int GetTotalFoodTrucksCount() => _foodTrucks.Count;

    public async Task<SearchResponse> SearchFoodTrucksAsync(SearchRequest request)
    {
        return await Task.Run(() =>
        {
            var filteredTrucks = _foodTrucks.AsEnumerable();

            // Filter by preferred food if specified
            if (!string.IsNullOrWhiteSpace(request.PreferredFood))
            {
                filteredTrucks = filteredTrucks.Where(ft =>
                    !string.IsNullOrWhiteSpace(ft.FoodItems) &&
                    ft.FoodItems.Contains(request.PreferredFood, StringComparison.OrdinalIgnoreCase));
            }

            // Filter only approved trucks
            filteredTrucks = filteredTrucks.Where(ft => 
                ft.Status.Equals("APPROVED", StringComparison.OrdinalIgnoreCase));

            // Calculate distances and sort
            var trucksWithDistance = filteredTrucks
                .Select(ft =>
                {
                    ft.Distance = CalculateDistance(
                        request.Latitude.Value, request.Longitude.Value,
                        ft.Latitude, ft.Longitude);
                    return ft;
                })
                .OrderBy(ft => ft.Distance)
                .Take(request.Limit)
                .ToList();

            _logger.LogInformation(
                "Search completed: Found {Count} food trucks for food='{Food}' near ({Lat}, {Lon})",
                trucksWithDistance.Count,
                request.PreferredFood ?? "any",
                request.Latitude.Value,
                request.Longitude.Value);

            return new SearchResponse
            {
                SearchLatitude = request.Latitude.Value,
                SearchLongitude = request.Longitude.Value,
                PreferredFood = request.PreferredFood,
                TotalResults = trucksWithDistance.Count,
                Results = [.. trucksWithDistance.Select(ft => new FoodTruckResult
                {
                    Name = ft.Applicant,
                    FacilityType = ft.FacilityType,
                    Address = ft.Address,
                    LocationDescription = ft.LocationDescription,
                    FoodItems = ft.FoodItems,
                    Status = ft.Status,
                    Latitude = ft.Latitude,
                    Longitude = ft.Longitude,
                    DistanceInMiles = Math.Round(ft.Distance ?? 0, 2),
                    Schedule = ft.Schedule
                })]
            };
        });
    }

    /// <summary>
    /// Calculate distance between two points using Haversine formula
    /// Returns distance in miles
    /// </summary>
    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusInMiles = 3959;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusInMiles * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}

/// <summary>
/// CSV record mapping class for CsvHelper
/// </summary>
/// <param name="Locationid"></param>
/// <param name="Applicant"></param>
/// <param name="FacilityType"></param>
/// <param name="LocationDescription"></param>
/// <param name="Address"></param>
/// <param name="Status"></param>
/// <param name="FoodItems"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="Schedule"></param>
public record FoodTruckCsvRecord(string? Locationid,
                                 string? Applicant,
                                 string? FacilityType,
                                 string? LocationDescription,
                                 string? Address,
                                 string? Status,
                                 string? FoodItems,
                                 double Latitude,
                                 double Longitude,
                                 string? Schedule);
