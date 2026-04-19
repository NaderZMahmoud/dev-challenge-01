namespace FoodTruckFinder.Models;

public class SearchResponse
{
    public double SearchLatitude { get; set; }
    public double SearchLongitude { get; set; }
    public string? PreferredFood { get; set; }
    public int TotalResults { get; set; }
    public List<FoodTruckResult> Results { get; set; } = [];
}

public class FoodTruckResult
{
    public string Name { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string LocationDescription { get; set; } = string.Empty;
    public string FoodItems { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double DistanceInMiles { get; set; }
    public string? Schedule { get; set; }
}
