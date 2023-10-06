using GeoIp.Data;

namespace GeoIp.Tests;

public class Search
{
    private readonly GeoIpDatabase dataBase;

    public Search()
    {
        dataBase = new GeoIpDatabase(File.ReadAllBytes("geobase.dat"));
    }

    [Fact]
    public void Search_by_city_name_exact()
    {
        string cityName = "cit_Epimyj";

        Location foundLocation = default;
        dataBase.FindLocationsByCity(cityName, StringSearchMode.Exact, location =>
        {
            foundLocation = location;
            return false;
        });

        var foundLocationManaged = foundLocation.GetManaged();
        Assert.NotNull(foundLocationManaged);
        Assert.Equal(cityName, foundLocationManaged.City);
    }
}