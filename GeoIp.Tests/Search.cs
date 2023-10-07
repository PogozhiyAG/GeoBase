using GeoIp.Data;

namespace GeoIp.Tests;

public class Search
{
    private readonly GeoIpDatabase dataBase;



    public Search()
    {
        dataBase = new GeoIpDatabase(File.ReadAllBytes("geobase.dat"));
    }

    [Theory]
    [InlineData("cit_Epimyj", StringSearchMode.Exact, true)]
    [InlineData("cit_Ep", StringSearchMode.Starts, true)]
    [InlineData("AAA", StringSearchMode.Exact, false)]
    [InlineData("AAA", StringSearchMode.Starts, false)]
    public void Search_by_city_name(string cityName, StringSearchMode searchMode, bool shouldBeFound)
    {
        int count = 0;
        dataBase.FindLocationsByCity(cityName, StringSearchMode.Exact, location =>
        {
            Assert.True(
                searchMode switch
                {
                    StringSearchMode.Exact => location.City == cityName,
                    StringSearchMode.Starts => location.City!.StartsWith(cityName),
                    _ => false
                }
            );
            count++;
            return true;
        });

        Assert.True(!shouldBeFound || count > 0);

    }
}