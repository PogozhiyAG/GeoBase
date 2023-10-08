namespace GeoIp.Tests;

public class Search : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture databaseFixture;

    public Search(DatabaseFixture databaseFixture)
    {
        this.databaseFixture = databaseFixture;
    }

    [Theory]
    [InlineData("cit_Epimyj", StringSearchMode.Exact, true)]
    [InlineData("cit_Ep", StringSearchMode.Starts, true)]
    [InlineData("AAA", StringSearchMode.Exact, false)]
    [InlineData("AAA", StringSearchMode.Starts, false)]
    public void Search_by_city_name(string cityName, StringSearchMode searchMode, bool shouldBeFound)
    {
        int count = 0;

        databaseFixture.Database.FindLocationsByCity(cityName, searchMode, location =>
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


public class DatabaseFixture
{
    public GeoIpDatabase Database { get; set; }

    public DatabaseFixture()
    {
        Database = new GeoIpDatabase(File.ReadAllBytes("geobase.dat"));
    }
}