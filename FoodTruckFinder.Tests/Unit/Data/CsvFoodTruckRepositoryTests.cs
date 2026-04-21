using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using FoodTruckFinder.Data;
using FoodTruckFinder.Tests.Fixtures;
using Xunit;

namespace FoodTruckFinder.Tests.Unit.Data;

public class CsvFoodTruckRepositoryTests
{
    private readonly Mock<ILogger<CsvFoodTruckRepository>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public CsvFoodTruckRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<CsvFoodTruckRepository>>();
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task Constructor_LoadsDataFromCsv_Successfully()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        try
        {
            // Act
            var repository = new CsvFoodTruckRepository(_mockLogger.Object, _mockConfiguration.Object);
            var count = await repository.GetCountAsync();

            // Assert
            count.Should().BeGreaterThan(0);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllLoadedTrucks()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        try
        {
            // Act
            var repository = new CsvFoodTruckRepository(_mockLogger.Object, _mockConfiguration.Object);
            var trucks = await repository.GetAllAsync();

            // Assert
            trucks.Should().NotBeEmpty();
            trucks.Count().Should().BeGreaterThan(0);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task GetByFoodTypeAsync_FiltersCorrectly()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        try
        {
            // Act
            var repository = new CsvFoodTruckRepository(_mockLogger.Object, _mockConfiguration.Object);
            var trucksByFood = await repository.GetByFoodTypeAsync("Tacos");

            // Assert
            trucksByFood.Should().NotBeEmpty();
            trucksByFood.All(x => x.FoodItems.Contains("Tacos", StringComparison.OrdinalIgnoreCase))
                .Should().BeTrue();
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task GetByFoodTypeAsync_WithNullOrEmptyFoodType_ReturnsAll()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        try
        {
            // Act
            var repository = new CsvFoodTruckRepository(_mockLogger.Object, _mockConfiguration.Object);
            var allTrucks = await repository.GetAllAsync();
            var trucksByNullFood = await repository.GetByFoodTypeAsync(null!);

            // Assert
            trucksByNullFood.Should().HaveSameCount(allTrucks);
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public async Task GetCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var csvPath = CreateTempCsvFile();
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns(csvPath);

        try
        {
            // Act
            var repository = new CsvFoodTruckRepository(_mockLogger.Object, _mockConfiguration.Object);
            var allTrucks = await repository.GetAllAsync();
            var count = await repository.GetCountAsync();

            // Assert
            count.Should().Be(allTrucks.Count());
        }
        finally
        {
            File.Delete(csvPath);
        }
    }

    [Fact]
    public void Constructor_WithMissingCsvFile_ReturnsEmptyRepository()
    {
        // Arrange
        _mockConfiguration
            .Setup(x => x["DataSource:CsvPath"])
            .Returns("/nonexistent/path/file.csv");

        // Act & Assert - should not throw
        var repository = new CsvFoodTruckRepository(_mockLogger.Object, _mockConfiguration.Object);
        repository.Should().NotBeNull();
    }

    private string CreateTempCsvFile()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"food_trucks_{Guid.NewGuid()}.csv");
        var csvData = TestData.GetSampleCsvData();
        File.WriteAllText(tempPath, csvData);
        return tempPath;
    }
}
