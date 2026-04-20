namespace FoodTruckFinder.Tests.Fixtures;

public class TestData
{
    /// <summary>
    /// Sample CSV data for testing food truck service (matches actual SF open data format)
    /// </summary>
    public static string GetSampleCsvData()
    {
        return @"locationid,Applicant,FacilityType,cnn,LocationDescription,Address,blocklot,block,lot,permit,Status,FoodItems,X,Y,Latitude,Longitude,Schedule,dayshours,NOISent,Approved,Received,PriorPermit,ExpirationDate,Location
1,Test Taco Truck,Truck,1,MARKET ST: BETWEEN A AND B,100 MARKET ST,0001001,0001,001,01MFF-00001,APPROVED,Tacos: California; Carne Asada,6007000,2104000,37.7749,-122.4194,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7749 -122.4194)
2,Pizza Palace,Truck,2,MARKET ST: BETWEEN B AND C,500 MARKET ST,0001002,0001,002,01MFF-00002,APPROVED,Pizza; Pasta,6007100,2104100,37.7758,-122.4182,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7758 -122.4182)
3,Thai Express,Truck,3,MARKET ST: BETWEEN C AND D,600 MARKET ST,0001003,0001,003,01MFF-00003,APPROVED,Thai: Pad Thai; Curry,6007200,2104200,37.7765,-122.4175,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7765 -122.4175)
4,Chinese Noodles,Truck,4,MARKET ST: BETWEEN D AND E,700 MARKET ST,0001004,0001,004,01MFF-00004,APPROVED,Chinese: Noodles; Fried Rice,6007300,2104300,37.7774,-122.4165,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7774 -122.4165)
5,Burger Barn,Truck,5,MARKET ST: BETWEEN E AND F,800 MARKET ST,0001005,0001,005,01MFF-00005,APPROVED,American: Burgers; Hot Dogs,6007400,2104400,37.7784,-122.4152,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7784 -122.4152)
6,Taco Truck 2,Truck,6,MARKET ST: BETWEEN F AND G,900 MARKET ST,0001006,0001,006,01MFF-00006,APPROVED,Tacos: Al Pastor; Carnitas,6007500,2104500,37.7795,-122.4140,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7795 -122.4140)
7,Vietnamese Sub,Truck,7,MARKET ST: BETWEEN G AND H,1000 MARKET ST,0001007,0001,007,01MFF-00007,APPROVED,Vietnamese: Pho; Banh Mi,6007600,2104600,37.7805,-122.4128,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7805 -122.4128)
8,Indian Spice,Truck,8,MARKET ST: BETWEEN H AND I,1100 MARKET ST,0001008,0001,008,01MFF-00008,APPROVED,Indian: Curry; Biryani,6007700,2104700,37.7815,-122.4115,http://example.com,,,09/20/2023,20230920,1,11/15/2024,(37.7815 -122.4115)
";
    }

    /// <summary>
    /// San Francisco downtown latitude/longitude for testing
    /// </summary>
    public static (double Latitude, double Longitude) GetSanFranciscoCentral() => (37.7749, -122.4194);

    /// <summary>
    /// Alternative San Francisco location for testing
    /// </summary>
    public static (double Latitude, double Longitude) GetSanFranciscoNorth() => (37.8044, -122.2712);

    /// <summary>
    /// Far away location for testing no results scenario
    /// </summary>
    public static (double Latitude, double Longitude) GetFarAwayLocation() => (37.3382, -121.8863);
}
