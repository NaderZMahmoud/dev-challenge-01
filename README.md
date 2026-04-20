# Food Truck Finder

## Overview

Food Truck Finder is an ASP.NET Core 8 Web API that helps users find food trucks near their location in San Francisco based on their preferred food type. The solution uses San Francisco's open food truck dataset and returns results ordered by proximity to the search location.

## Features

* **Location-based Search**: Find food trucks near a specified latitude and longitude
* **Food Type Filtering**: Filter results by preferred food type (case-insensitive)
* **Configurable Results**: Request between 1-100 food truck results
* **Distance Ordering**: Results are automatically sorted by distance from the search location
* **Data Source**: Powered by San Francisco's open food truck dataset
* **API Documentation**: Interactive Swagger/OpenAPI documentation
* **CORS Support**: Configured to accept requests from any origin
* **Comprehensive Logging**: Console and debug logging for monitoring

## Project Structure

```
FoodTruckFinder/
├── Controllers/
│   └── FoodTrucksController.cs       # API endpoints for food truck search
├── Models/
│   ├── FoodTruck.cs                  # Food truck data model
│   ├── SearchRequest.cs              # Search query parameters
│   └── SearchResponse.cs             # Search result response
├── Services/
│   └── FoodTruckService.cs           # Business logic for food truck search
├── Data/
│   └── Mobile_Food_Facility_Permit.csv  # San Francisco food truck dataset
├── Program.cs                        # Application configuration
├── FoodTruckFinder.csproj            # Project file
└── FoodTruckFinder.http              # HTTP request samples
```

## API Endpoints

### Search Food Trucks
**POST** `/api/foodtrucks/search`

#### Request Body
```json
{
  "latitude": 37.7749,
  "longitude": -122.4194,
  "limit": 10,
  "preferredFood": "taco"
}
```

#### Request Parameters
- `latitude` (required): Latitude of search location (-90 to 90)
- `longitude` (required): Longitude of search location (-180 to 180)
- `limit` (optional): Maximum number of results to return (1-100, default: 5)
- `preferredFood` (optional): Food type to filter by (case-insensitive)

#### Response
```json
{
  "results": [
    {
      "id": "string",
      "name": "string",
      "foodItems": "string",
      "latitude": 0.0,
      "longitude": 0.0,
      "distance": 0.0
    }
  ],
  "totalResults": 0,
  "searchLocation": {
    "latitude": 0.0,
    "longitude": 0.0
  }
}
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio Code or Visual Studio

### Running the Application
1. Navigate to the project directory:
   ```bash
   cd FoodTruckFinder/FoodTruckFinder
   ```

2. Build the solution:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. The API will be available at `http://localhost:5000` (or the configured port)
5. Swagger documentation is available at `http://localhost:5000/swagger`

## Testing

### Using the HTTP File
Sample requests are provided in `FoodTruckFinder.http` for testing with REST Client extensions.

### Using Swagger UI
Navigate to `/swagger` to access the interactive API documentation where you can test endpoints directly.

### Example Request
```bash
curl -X POST http://localhost:5000/api/foodtrucks/search \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 37.7749,
    "longitude": -122.4194,
    "limit": 10,
    "preferredFood": "taco"
  }'
```

## Data Source

The food truck data is sourced from the San Francisco open dataset located in `Data/Mobile_Food_Facility_Permit.csv`. The dataset is loaded into memory as a singleton service at application startup for optimal query performance.

## Technical Details

- **Framework**: ASP.NET Core 8
- **Data Format**: CSV (in-memory dataset)
- **Dependency Injection**: IFoodTruckService with singleton lifetime
- **Validation**: FluentValidation-style attribute validation
- **Documentation**: Swagger/OpenAPI with XML comments
- **CORS**: Configured to allow requests from any origin
- **Logging**: Integrated with ILogger and console/debug providers

## Architecture Notes

- The solution loads all food truck data into memory at startup for quick searches
- Distance calculations use the Haversine formula for accurate geographic distance
- Search results are filtered and sorted in-memory, making it read-only as required
- No database is required for this proof of concept

