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
        var searchResults = new List<Location.Managed>();

        dataBase.FindLocationsByCity(cityName, StringSearchMode.Exact, location =>
        {
            searchResults.Add(location);
            return true;
        });

        Assert.True(searchResults.Any());
        Assert.True(searchResults.TrueForAll(l => l.City == cityName));
    }
}