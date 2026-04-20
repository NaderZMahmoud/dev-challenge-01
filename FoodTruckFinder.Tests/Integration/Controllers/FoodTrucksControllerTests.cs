using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FoodTruckFinder;
using FoodTruckFinder.Models;
using FoodTruckFinder.Tests.Fixtures;
using Xunit;

namespace FoodTruckFinder.Tests.Integration.Controllers;

public class FoodTrucksControllerTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithValidRequest_ReturnsOkWithResults()
    {
        // Arrange
        var request = FoodTruckFixtures.CreateSearchRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var jsonString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<SearchResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        content.Should().NotBeNull();
        content!.Results.Should().NotBeEmpty();
        content.TotalResults.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithMissingLatitude_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new { longitude = -122.4194, limit = 5 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithMissingLongitude_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new { latitude = 37.7749, limit = 5 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithInvalidLatitude_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new { latitude = 91.0, longitude = -122.4194, limit = 5 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithInvalidLongitude_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new { latitude = 37.7749, longitude = -181.0, limit = 5 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithLimitZero_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new { latitude = 37.7749, longitude = -122.4194, limit = 0 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithLimitGreaterThan100_ReturnsBadRequest()
    {
        // Arrange
        var invalidRequest = new { latitude = 37.7749, longitude = -122.4194, limit = 101 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithValidLimitRange_ReturnsOk()
    {
        // Arrange & Act & Assert
        for (int limit = 1; limit <= 100; limit += 10)
        {
            var request = FoodTruckFixtures.CreateSearchRequest(limit: limit);
            var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithFoodFilter_ReturnsFilteredResults()
    {
        // Arrange
        var request = FoodTruckFixtures.CreateSearchRequest(preferredFood: "Tacos");

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var jsonString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<SearchResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        content!.Results.All(x => x.FoodItems.Contains("Tacos", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue();
    }

    [Fact]
    public async Task Post_SearchEndpoint_ResponseHasCorrectStructure()
    {
        // Arrange
        var request = FoodTruckFixtures.CreateSearchRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", request);
        var jsonString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<SearchResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        content!.Results.Should().NotBeNull();
        content.TotalResults.Should().BeGreaterThanOrEqualTo(0);

        // Check individual food truck properties
        foreach (var truck in content.Results)
        {
            truck.Name.Should().NotBeNullOrEmpty();
            truck.Latitude.Should().BeGreaterThanOrEqualTo(-90).And.BeLessThanOrEqualTo(90);
            truck.Longitude.Should().BeGreaterThanOrEqualTo(-180).And.BeLessThanOrEqualTo(180);
            truck.DistanceInMiles.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    [Fact]
    public async Task Post_SearchEndpoint_ReturnsJsonContentType()
    {
        // Arrange
        var request = FoodTruckFixtures.CreateSearchRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", request);

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task Post_SearchEndpoint_WithEmptyBody_ReturnsBadRequest()
    {
        // Act
        var response = await _client.PostAsync("/api/foodtrucks/search", 
            new StringContent("", System.Text.Encoding.UTF8, "application/json"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Post_SearchEndpoint_WithVariousLimits_ReturnsCorrectCount(int limit)
    {
        // Arrange
        var request = FoodTruckFixtures.CreateSearchRequest(limit: limit);

        // Act
        var response = await _client.PostAsJsonAsync("/api/foodtrucks/search", request);
        var jsonString = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<SearchResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content!.Results.Count.Should().BeLessThanOrEqualTo(limit);
    }

    [Fact]
    public async Task Get_SwaggerEndpoint_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/swagger/index.html");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_SwaggerApiDocs_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
