# Food Truck Finder

## Overview

Food Truck Finder is an ASP.NET Core 8 Web API that helps users find food trucks near their location in San Francisco based on their preferred food type. The solution uses San Francisco's open food truck dataset and returns results ordered by proximity to the search location. It includes comprehensive unit and integration tests for quality assurance.

## Features

* **Location-based Search**: Find food trucks near a specified latitude and longitude
* **Multiple Search Methods**: Use POST with JSON body or GET with query parameters
* **Food Type Filtering**: Filter results by preferred food type (case-insensitive)
* **Configurable Results**: Request between 1-100 food truck results
* **Distance Ordering**: Results are automatically sorted by distance from the search location
* **Coordinate Validation**: Strict validation ensures valid latitude (-90 to 90) and longitude (-180 to 180)
* **Health Endpoint**: Check API status and total food trucks in the dataset
* **Data Source**: Powered by San Francisco's open food truck dataset
* **API Documentation**: Interactive Swagger/OpenAPI documentation
* **CORS Support**: Configured to accept requests from any origin
* **Comprehensive Logging**: Console and debug logging for monitoring
* **Test Suite**: Full unit and integration test coverage with 28 tests

## Project Structure

```
dev-challenge-01/
├── FoodTruckFinder/
│   └── FoodTruckFinder/
│       ├── Controllers/
│       │   └── FoodTrucksController.cs       # API endpoints for food truck search
│       ├── Models/
│       │   ├── FoodTruck.cs                  # Food truck data model
│       │   ├── SearchRequest.cs              # Search query parameters
│       │   └── SearchResponse.cs             # Search result response
│       ├── Services/
       │   ├── FoodTruckService.cs           # Business logic for food truck search
       │   └── Models/
       │       └── FoodTruckSearchQuery.cs   # Search query domain model
│       ├── Data/
│       │   └── Mobile_Food_Facility_Permit.csv  # San Francisco food truck dataset
│       ├── Program.cs                        # Application configuration
│       ├── FoodTruckFinder.csproj            # Project file
│       └── FoodTruckFinder.http              # HTTP request samples
├── FoodTruckFinder.Tests/
│   ├── Unit/Services/
│   │   └── FoodTruckServiceTests.cs          # Unit tests for service layer
│   ├── Integration/Controllers/
│   │   └── FoodTrucksControllerTests.cs      # Integration tests for API endpoints
│   ├── Fixtures/
│   │   ├── FoodTruckFixtures.cs              # Test data builders
│   │   └── TestData.cs                       # Sample CSV data
│   └── FoodTruckFinder.Tests.csproj          # Test project file
├── dev-challenge-01.sln                      # Solution file
├── README.md                                 # This file
└── .devcontainer/
    └── devcontainer.json                    # Dev container configuration
```

## API Endpoints

### 1. Search Food Trucks (POST with JSON)
**POST** `/api/foodtrucks/search`

Search for food trucks using a JSON request body.

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

#### Response (200 OK)
```json
{
  "searchLatitude": 37.7749,
  "searchLongitude": -122.4194,
  "preferredFood": "taco",
  "totalResults": 5,
  "results": [
    {
      "name": "Taco Truck 1",
      "facilityType": "Truck",
      "address": "100 Market St",
      "locationDescription": "MARKET ST: Between 1st and 2nd",
      "foodItems": "Tacos: California, Carne Asada",
      "status": "APPROVED",
      "latitude": 37.7749,
      "longitude": -122.4194,
      "distanceInMiles": 0.0,
      "schedule": "http://example.com/schedule"
    }
  ]
}
```

#### Error Response (400 Bad Request)
```json
{
  "error": "Latitude must be between -90 and 90"
}
```

### 2. Search Food Trucks (GET with Query Parameters)
**GET** `/api/foodtrucks/search?latitude=37.7749&longitude=-122.4194&limit=10&preferredFood=taco`

Search for food trucks using query parameters. Useful for simple REST calls and browser testing.

#### Query Parameters
- `latitude` (required): Latitude of search location (-90 to 90)
- `longitude` (required): Longitude of search location (-180 to 180)
- `limit` (optional): Maximum number of results to return (1-100, default: 5)
- `preferredFood` (optional): Food type to filter by (case-insensitive)

#### Response
Same as POST search endpoint

### 3. Health Check
**GET** `/api/foodtrucks/health`

Get API health status and statistics.

#### Response (200 OK)
```json
{
  "status": "healthy",
  "totalFoodTrucks": 456,
  "timestamp": "2024-04-21T10:30:00Z"
}
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio Code or Visual Studio
- Git

### Setup Options

#### Option 1: Using Dev Container (Recommended)
1. Open the workspace in VS Code with Dev Containers extension
2. Click "Reopen in Container" when prompted
3. The container will automatically restore and build the solution
4. All dependencies are pre-installed

#### Option 2: Local Setup
1. Clone or navigate to the project directory:
   ```bash
   cd /workspaces/dev-challenge-01
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the solution:
   ```bash
   dotnet build
   ```

### Running the Application
1. Navigate to the API project:
   ```bash
   cd FoodTruckFinder/FoodTruckFinder
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. The API will be available at:
   - HTTP: `http://localhost:5187`
   - HTTPS: `https://localhost:7100`

4. Swagger documentation is available at `http://localhost:5187/swagger`

## Testing

### Running All Tests
```bash
cd /workspaces/dev-challenge-01
dotnet test
```

### Running Unit Tests Only
```bash
dotnet test --filter "Unit"
```

### Running Integration Tests Only
```bash
dotnet test --filter "Integration"
```

### Test Coverage
- **Unit Tests (11 tests)**:
  - CSV loading and data initialization
  - Search functionality with various parameters
  - Food type filtering (case-insensitive)
  - Result limit validation
  - Distance-based sorting
  - Edge cases and boundary conditions

- **Integration Tests (17 tests)**:
  - Valid search requests
  - Request validation (coordinate and limit constraints)
  - Food filter functionality
  - Response structure validation
  - Various limit values
  - Error handling (400, 500 responses)
  - Swagger documentation endpoints

### Using the HTTP File
Sample requests are provided in [FoodTruckFinder.http](FoodTruckFinder/FoodTruckFinder/FoodTruckFinder.http) for testing with REST Client extensions.

### Using Swagger UI
Navigate to `/swagger` to access the interactive API documentation where you can test endpoints directly.

### Example curl Requests
```bash
# POST search
curl -X POST http://localhost:5187/api/foodtrucks/search \
  -H "Content-Type: application/json" \
  -d '{
    "latitude": 37.7749,
    "longitude": -122.4194,
    "limit": 10,
    "preferredFood": "taco"
  }'

# GET search with query parameters
curl "http://localhost:5187/api/foodtrucks/search?latitude=37.7749&longitude=-122.4194&limit=5&preferredFood=pizza"

# Health check
curl http://localhost:5187/api/foodtrucks/health
```

## Data Source

The food truck data is sourced from the San Francisco open dataset located in [Data/Mobile_Food_Facility_Permit.csv](FoodTruckFinder/FoodTruckFinder/Data/Mobile_Food_Facility_Permit.csv). The dataset includes:
- Food truck name and applicant information
- Facility type and status
- Location (address, coordinates, location description)
- Food items served
- Operating schedule

The dataset is loaded into memory as a singleton service at application startup for optimal query performance.

## Technical Details

- **Framework**: ASP.NET Core 8
- **Language**: C#
- **Data Format**: CSV (in-memory dataset, no database required)
- **Testing Framework**: xUnit with Moq and FluentAssertions
- **Dependency Injection**: IFoodTruckService with singleton lifetime
- **Validation**: Data annotation attributes with explicit coordinate validation
- **Documentation**: Swagger/OpenAPI with XML comments
- **CORS**: Configured to allow requests from any origin
- **Logging**: Integrated with ILogger (console and debug providers)
- **Dev Container**: .NET 8, Git, Azure CLI, VS Code extensions

## Architecture Notes

- **In-Memory Dataset**: All food truck data is loaded into memory at startup for O(1) access and fast searches
- **Distance Calculation**: Uses the Haversine formula for accurate geographic distance in miles
- **Search Logic**: Filters and sorts results in-memory, making the API read-only as required
- **Validation Layers**: 
  - Declarative validation via `[Range]` attributes
  - Explicit coordinate validation in controller
  - ModelState validation for complete request validation
- **No Database**: This proof of concept uses no external database; all data is sourced from CSV

## Development Workflow

1. **Writing Code**: Use VS Code with C# extensions for development
2. **Building**: Use `dotnet build` to compile
3. **Running**: Use `dotnet run` to start the API
4. **Testing**: Use `dotnet test` to run the test suite
5. **Debugging**: Use VS Code debugger with breakpoints and watch expressions

## Troubleshooting

### Port Already in Use
If port 5187/7100 is already in use, specify a different port:
```bash
dotnet run -- --urls "http://localhost:5005"
```

### CSV File Not Found
Ensure the CSV file path is correctly configured in `appsettings.json`:
```json
{
  "DataSource": {
    "CsvPath": "Data/Mobile_Food_Facility_Permit.csv"
  }
}
```

### Test Failures
Ensure the project builds successfully and dependencies are restored:
```bash
dotnet restore
dotnet build
dotnet test -v normal
```

