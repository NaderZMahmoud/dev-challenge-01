namespace FoodTruckFinder.Models;

public class FoodTruck
{
    public string LocationId { get; set; } = string.Empty;
    public string Applicant { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty;
    public string LocationDescription { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string FoodItems { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Schedule { get; set; } = string.Empty;
    public double? Distance { get; set; }
}
