using CsvHelper;
using CsvHelper.Configuration;
using FoodTruckFinder.Constants;
using FoodTruckFinder.Models;
using FoodTruckFinder.Services;
using System.Globalization;

namespace FoodTruckFinder.Data;

/// <summary>
/// CSV-based implementation of the food truck repository.
/// Loads food truck data from a CSV file and provides query capabilities.
/// </summary>
public class CsvFoodTruckRepository : IFoodTruckRepository
{
    private readonly List<FoodTruck> _foodTrucks;
    private readonly ILogger<CsvFoodTruckRepository> _logger;

    public CsvFoodTruckRepository(ILogger<CsvFoodTruckRepository> logger, IConfiguration configuration)
    {
        _logger = logger;
        _foodTrucks = LoadFoodTrucksFromCsv(configuration);
        _logger.LogInformation("Repository initialized: Loaded {Count} food trucks from CSV", _foodTrucks.Count);
    }

    public Task<IEnumerable<FoodTruck>> GetAllAsync()
    {
        return Task.FromResult(_foodTrucks.AsEnumerable());
    }

    public Task<IEnumerable<FoodTruck>> GetByFoodTypeAsync(string foodType)
    {
        if (string.IsNullOrWhiteSpace(foodType))
        {
            return Task.FromResult(_foodTrucks.AsEnumerable());
        }

        var results = _foodTrucks.Where(ft =>
            !string.IsNullOrWhiteSpace(ft.FoodItems) &&
            ft.FoodItems.Contains(foodType, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(results.AsEnumerable());
    }

    public Task<int> GetCountAsync()
    {
        return Task.FromResult(_foodTrucks.Count);
    }

    /// <summary>
    /// Loads food truck data from a CSV file configured in appsettings.
    /// </summary>
    /// <param name="configuration">Application configuration containing the CSV path.</param>
    /// <returns>A list of loaded food trucks, or an empty list if loading fails.</returns>
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
}
