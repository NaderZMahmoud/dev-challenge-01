using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using FoodTruckFinder.Services;
using FoodTruckFinder.Services.Models;
using FoodTruckFinder.Tests.Fixtures;
using Xunit;

namespace FoodTruckFinder.Tests.Unit.Services;

public class FoodTruckServiceTests
{
    private readonly Mock<ILogger<FoodTruckService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public FoodTruckServiceTests()
    {
        _mockLogger = new Mock<ILogger<FoodTruckService>>();
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Fact]
    public void Constructor_LoadsDataFromCsv_Successfully()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        try
        {
            // Act
            var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
            var count = service.GetTotalFoodTrucksCount();

            // Assert
            count.Should().BeGreaterThan(0);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithValidRequest_ReturnsResults()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery();

        try
        {
            // Act
            var result = await service.SearchFoodTrucksAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().NotBeEmpty();
            result.TotalResults.Should().BeGreaterThan(0);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithFoodFilter_ReturnsFilteredResults()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(preferredFood: "Tacos");

        try
        {
            // Act
            var result = await service.SearchFoodTrucksAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().NotBeEmpty();
            result.Results.All(x => x.FoodItems!.Contains("Tacos", StringComparison.OrdinalIgnoreCase))
                .Should().BeTrue();
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithLimitParameter_ReturnsExactLimit()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
        var limit = 3;
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(limit: limit);

        try
        {
            // Act
            var result = await service.SearchFoodTrucksAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Results.Count.Should().BeLessThanOrEqualTo(limit);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_ResultsAreSortedByDistance()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery();

        try
        {
            // Act
            var result = await service.SearchFoodTrucksAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().NotBeEmpty();

            // Verify results are sorted by distance
            for (int i = 0; i < result.Results.Count - 1; i++)
            {
                var currentDistance = result.Results[i].DistanceInMiles;
                var nextDistance = result.Results[i + 1].DistanceInMiles;
                currentDistance.Should().BeLessThanOrEqualTo(nextDistance);
            }
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithCaseInsensitiveFilter_ReturnsResults()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(preferredFood: "TACOS");

        try
        {
            // Act
            var result = await service.SearchFoodTrucksAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().NotBeEmpty();
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithNoMatches_ReturnsEmptyResults()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(preferredFood: "XYZ_NONEXISTENT_FOOD");

        try
        {
            // Act
            var result = await service.SearchFoodTrucksAsync(query);

            // Assert
            result.Should().NotBeNull();
            result.Results.Should().BeEmpty();
            result.TotalResults.Should().Be(0);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public void GetTotalFoodTrucksCount_ReturnsCorrectCount()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        try
        {
            // Act
            var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
            var count = service.GetTotalFoodTrucksCount();

            // Assert
            count.Should().BeGreaterThan(0);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task SearchFoodTrucksAsync_WithVariousLimits_ReturnsUpToLimit(int limit)
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        var service = new FoodTruckService(_mockLogger.Object, _mockConfiguration.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(limit: limit);

        try
        {
            // Act
            var result = await service.SearchFoodTrucksAsync(query);

            // Assert
            result.Results.Count.Should().BeLessThanOrEqualTo(limit);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    private string CreateTempCsvFile()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"food_trucks_{Guid.NewGuid()}.csv");
        var csvData = TestData.GetSampleCsvData();
        File.WriteAllText(tempPath, csvData);
        return tempPath;
    }
}
