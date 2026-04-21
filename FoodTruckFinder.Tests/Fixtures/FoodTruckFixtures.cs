using System;
using System.Collections.Generic;
using FoodTruckFinder.Models;
using FoodTruckFinder.Services.Models;

namespace FoodTruckFinder.Tests.Fixtures;

public class FoodTruckFixtures
{
    public static FoodTruck CreateFoodTruck(
        string locationId = "1",
        string applicant = "Test Taco Truck",
        string foodItems = "Tacos: California, Carne Asada",
        double latitude = 37.7749,
        double longitude = -122.4194)
    {
        return new FoodTruck
        {
            LocationId = locationId,
            Applicant = applicant,
            FacilityType = "Truck",
            Address = "100 Test St",
            LocationDescription = "Test Location",
            Status = "APPROVED",
            FoodItems = foodItems,
            Latitude = latitude,
            Longitude = longitude,
            Schedule = "Day"
        };
    }

    public static List<FoodTruck> CreateFoodTruckList(int count = 5)
    {
        var trucks = new List<FoodTruck>();
        for (int i = 0; i < count; i++)
        {
            trucks.Add(new FoodTruck
            {
                LocationId = (i + 1).ToString(),
                Applicant = $"Food Truck {i + 1}",
                FacilityType = "Truck",
                Address = $"{100 + i} Test St",
                LocationDescription = "Test Location",
                Status = "APPROVED",
                FoodItems = GetFoodTypeByIndex(i),
                Latitude = 37.7749 + (i * 0.001),
                Longitude = -122.4194 + (i * 0.001),
                Schedule = "Day"
            });
        }
        return trucks;
    }

    public static SearchRequest CreateSearchRequest(
        double latitude = 37.7749,
        double longitude = -122.4194,
        int limit = 5,
        string? preferredFood = null)
    {
        return new SearchRequest
        {
            Latitude = latitude,
            Longitude = longitude,
            Limit = limit,
            PreferredFood = preferredFood
        };
    }

    public static FoodTruckSearchQuery CreateFoodTruckSearchQuery(
        double latitude = 37.7749,
        double longitude = -122.4194,
        int limit = 5,
        string? preferredFood = null)
    {
        return new FoodTruckSearchQuery(latitude, longitude, limit, preferredFood);
    }

    public static SearchResponse CreateSearchResponse(
        List<FoodTruckResult>? results = null,
        double searchLatitude = 37.7749,
        double searchLongitude = -122.4194)
    {
        results ??= CreateFoodTruckResultList(3);
        
        return new SearchResponse
        {
            Results = results,
            TotalResults = results.Count,
            SearchLatitude = searchLatitude,
            SearchLongitude = searchLongitude
        };
    }

    public static List<FoodTruckResult> CreateFoodTruckResultList(int count = 5)
    {
        var trucks = new List<FoodTruckResult>();
        for (int i = 0; i < count; i++)
        {
            trucks.Add(new FoodTruckResult
            {
                Name = $"Food Truck {i + 1}",
                FacilityType = "Truck",
                Address = $"{100 + i} Test St",
                LocationDescription = "Test Location",
                Status = "APPROVED",
                FoodItems = GetFoodTypeByIndex(i),
                Latitude = 37.7749 + (i * 0.001),
                Longitude = -122.4194 + (i * 0.001),
                DistanceInMiles = i * 0.5,
                Schedule = "Day"
            });
        }
        return trucks;
    }

    private static string GetFoodTypeByIndex(int index)
    {
        return index switch
        {
            0 => "Tacos: California, Carne Asada",
            1 => "Chinese: Noodles, Fried Rice",
            2 => "Thai: Pad Thai, Curry",
            3 => "Italian: Pizza, Pasta",
            _ => "American: Burgers, Hot Dogs"
        };
    }
}
