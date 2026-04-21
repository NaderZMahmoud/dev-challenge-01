using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using FoodTruckFinder.Data;
using FoodTruckFinder.Models;
using FoodTruckFinder.Services;
using FoodTruckFinder.Services.Models;
using FoodTruckFinder.Tests.Fixtures;
using Xunit;

namespace FoodTruckFinder.Tests.Unit.Services;

public class FoodTruckServiceTests
{
    private readonly Mock<ILogger<FoodTruckService>> _mockLogger;
    private readonly Mock<IFoodTruckRepository> _mockRepository;

    public FoodTruckServiceTests()
    {
        _mockLogger = new Mock<ILogger<FoodTruckService>>();
        _mockRepository = new Mock<IFoodTruckRepository>();
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithValidRequest_ReturnsResults()
    {
        // Arrange
        var testTrucks = FoodTruckFixtures.CreateFoodTruckList(5);
        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(testTrucks);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery();

        // Act
        var result = await service.SearchFoodTrucksAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
        result.TotalResults.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithFoodFilter_ReturnsFilteredResults()
    {
        // Arrange
        var testTrucks = new List<FoodTruck>
        {
            FoodTruckFixtures.CreateFoodTruck(applicant: "Taco Truck 1", foodItems: "Tacos: Carnitas, Al Pastor"),
            FoodTruckFixtures.CreateFoodTruck(applicant: "Chinese Food", foodItems: "Chinese: Kung Pao, Lo Mein"),
            FoodTruckFixtures.CreateFoodTruck(applicant: "Taco Truck 2", foodItems: "Tacos: Fish, Carne Asada")
        };

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(testTrucks);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(preferredFood: "Tacos");

        // Act
        var result = await service.SearchFoodTrucksAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
        result.Results.All(x => x.FoodItems!.Contains("Tacos", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithLimitParameter_ReturnsExactLimit()
    {
        // Arrange
        var testTrucks = FoodTruckFixtures.CreateFoodTruckList(10);
        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(testTrucks);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);
        var limit = 3;
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(limit: limit);

        // Act
        var result = await service.SearchFoodTrucksAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Results.Count.Should().BeLessThanOrEqualTo(limit);
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_ResultsAreSortedByDistance()
    {
        // Arrange
        var testTrucks = FoodTruckFixtures.CreateFoodTruckList(5);
        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(testTrucks);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery();

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

    [Fact]
    public async Task SearchFoodTrucksAsync_WithCaseInsensitiveFilter_ReturnsResults()
    {
        // Arrange
        var testTrucks = FoodTruckFixtures.CreateFoodTruckList(5);
        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(testTrucks);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(preferredFood: "TACOS");

        // Act
        var result = await service.SearchFoodTrucksAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SearchFoodTrucksAsync_WithNoMatches_ReturnsEmptyResults()
    {
        // Arrange
        var testTrucks = FoodTruckFixtures.CreateFoodTruckList(5);
        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(testTrucks);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(preferredFood: "XYZ_NONEXISTENT_FOOD");

        // Act
        var result = await service.SearchFoodTrucksAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Results.Should().BeEmpty();
        result.TotalResults.Should().Be(0);
    }

    [Fact]
    public async Task GetTotalFoodTrucksCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        const int expectedCount = 42;
        _mockRepository
            .Setup(x => x.GetCountAsync())
            .ReturnsAsync(expectedCount);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);

        // Act
        var count = await service.GetTotalFoodTrucksCountAsync();

        // Assert
        count.Should().Be(expectedCount);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task SearchFoodTrucksAsync_WithVariousLimits_ReturnsUpToLimit(int limit)
    {
        // Arrange
        var testTrucks = FoodTruckFixtures.CreateFoodTruckList(20);
        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(testTrucks);

        var service = new FoodTruckService(_mockLogger.Object, _mockRepository.Object);
        var query = FoodTruckFixtures.CreateFoodTruckSearchQuery(limit: limit);

        // Act
        var result = await service.SearchFoodTrucksAsync(query);

        // Assert
        result.Results.Count.Should().BeLessThanOrEqualTo(limit);
    }
}
