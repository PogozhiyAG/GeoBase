using GeoIp.Data;
using GeoIp.Utils;

namespace GeoIp;

public enum StringSearchMode
{
    Exact,
    Starts
}

public enum BinarySearchEdgeMode
{
    Lower,
    Upper
}

/// <summary>
/// Database of IP address distribution by geographic location
/// </summary>
public unsafe class GeoIpDatabase
{
    /// <summary>
    /// Database in binary format
    /// </summary>
    private byte[]? rawData;

    /// <summary>
    /// Load data from from file
    /// </summary>
    /// <param name="fileName">database file name</param>
    public void LoadFromFile(string fileName)
    {
        rawData = File.ReadAllBytes(fileName);
    }

    /// <summary>
    /// Find location by specified IP address
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    /// <returns>found location</returns>
    public Location? FindLocationByIpAddress(uint ipAddress)
    {
        fixed (byte* baseAddress = rawData)
        {
            DataBaseHeader* dataBaseHeader = (DataBaseHeader*)baseAddress;
            IpAddressRange* range = (IpAddressRange*)(baseAddress + dataBaseHeader->OffsetRanges);
            Location* location = (Location*)(baseAddress + dataBaseHeader->OffsetLocations);

            int left = 0;
            int right = dataBaseHeader->Records - 1;

            //the binary search algorithm
            while (left <= right)
            {
                int current = (left + right) / 2;

                IpAddressRange* rangeMiddle = range + current;

                if (ipAddress >= rangeMiddle->From && ipAddress <= rangeMiddle->To)
                {
                    return *(location + current);
                }
                else if (ipAddress < rangeMiddle->From)
                {
                    right = current - 1;
                }
                else
                {
                    left = current + 1;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Find index of location by specified city name
    /// </summary>
    /// <param name="cityAscii">ASCII encoded city name</param>
    /// <param name="length">Effective search string length</param>
    /// <param name="from">Start index of the search range; -1 to search from the beginning</param>
    /// <param name="to">End index of th search range; -1 to search to the end</param>
    /// <param name="edgeMode">Edge search mode; Lower to search the first entry; Upper to search the last entry</param>
    /// <returns>index of found location</returns>
    public int FindLocationIndexByCity(byte[] cityAscii, int length, int from, int to, BinarySearchEdgeMode edgeMode)
    {
        fixed (byte* city = cityAscii)
        fixed (byte* baseAddress = rawData)
        {
            DataBaseHeader* dataBaseHeader = (DataBaseHeader*)baseAddress;
            Location* location = (Location*)(baseAddress + dataBaseHeader->OffsetLocations);
            uint* locationCityIndex = (uint*)(baseAddress + dataBaseHeader->OffsetCities);

            Location* currentLocation;

            if (from < 0) from = 0;
            if (to < 0) to = dataBaseHeader->Records - 1;

            int left = from;
            int right = to;
            int direction = edgeMode == BinarySearchEdgeMode.Upper ? +1 : -1;

            //the binary search algorithm
            while (left <= right)
            {
                int current = (left + right) / 2;

                currentLocation = (Location*)((byte*)location + *(locationCityIndex + current));
                int cmp = ASCIIStringUtils.CompareASCIIStrings(city, currentLocation->city, length);

                if (cmp == 0 && (edgeMode == BinarySearchEdgeMode.Upper ? current < to : current > from))
                {
                    //check the neighbour; if it fits the condition then continue the search
                    currentLocation = (Location*)((byte*)location + *(locationCityIndex + current + direction));
                    int cmpPrev = ASCIIStringUtils.CompareASCIIStrings(city, currentLocation->city, length);
                    if (cmpPrev == 0)
                    {
                        cmp = direction;
                    }
                }

                if (cmp == 0)
                {
                    return current;
                }
                else if (cmp < 0)
                {
                    right = current - 1;
                }
                else
                {
                    left = current + 1;
                }
            }

            return -1;
        }
    }

    /// <summary>
    /// Find location by specified city name
    /// </summary>
    /// <param name="city">Еhe city name</param>
    /// <param name="searchMode">Search mode</param>
    /// <param name="onFoundLocationDelegate">The delegate that will be called for each location found; If this delegate returns false, enumeration of the results will be finished</param>
    /// <exception cref="ArgumentException"></exception>
    public void FindLocationsByCity(string city, StringSearchMode searchMode, Func<Location, bool> onFoundLocationDelegate)
    {
        int stringLength = searchMode switch
        {
            StringSearchMode.Exact => Location.LENGTH_CITY,
            StringSearchMode.Starts => city.Length,
            _ => throw new ArgumentException($"The search mode must be a {nameof(StringSearchMode)} value", nameof(searchMode))
        };

        byte[] cityAscii = ASCIIStringUtils.GetASCIIFromString(city);

        int from = FindLocationIndexByCity(cityAscii, stringLength, 0, -1, BinarySearchEdgeMode.Lower);
        if (from >= 0)
        {
            int to = FindLocationIndexByCity(cityAscii, stringLength, from, -1, BinarySearchEdgeMode.Upper);

            //results enumeration
            fixed (byte* baseAddress = rawData)
            {
                DataBaseHeader* dataBaseHeader = (DataBaseHeader*)baseAddress;
                Location* location = (Location*)(baseAddress + dataBaseHeader->OffsetLocations);
                uint* locationCityIndex = (uint*)(baseAddress + dataBaseHeader->OffsetCities);

                while (from <= to)
                {
                    var currentLocation = *(Location*)((byte*)location + *(locationCityIndex + from));
                    from++;
                    if (!onFoundLocationDelegate(currentLocation))
                    {
                        break;
                    }
                }
            }
        }
    }

    public int GetRecordCount()
    {
        fixed (byte* baseAddress = rawData)
        {
            return ((DataBaseHeader*)baseAddress)->Records;
        }
    }
}
