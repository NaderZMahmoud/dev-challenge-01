namespace FoodTruckFinder.Services;

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
