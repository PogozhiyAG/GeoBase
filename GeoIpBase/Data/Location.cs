using System.Runtime.InteropServices;
using GeoIp.Utils;

namespace GeoIp.Data;

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public unsafe struct Location
{
    public const int LENGTH_CITY = 24;

    [FieldOffset(0)] public fixed byte country[8];          // country name (random string with prefix "cou_")
    [FieldOffset(8)] public fixed byte region[12];          // region name (random string with prefix "reg_")
    [FieldOffset(20)] public fixed byte postal[12];         // post code (random string with prefix "pos_")
    [FieldOffset(32)] public fixed byte city[LENGTH_CITY];  // city name (random string with prefix "cit_")
    [FieldOffset(56)] public fixed byte organization[32];   // organization name (random string with prefix  "org_")
    [FieldOffset(88)] public float latitude;                // position latitude
    [FieldOffset(92)] public float longitude;               // position longitude


    public Managed GetManaged()
    {
        Managed result = new Managed();

        fixed (byte* b = country) result.Country = ASCIIStringUtils.GetStringFromASCII(b, 24);
        fixed (byte* b = region) result.Region = ASCIIStringUtils.GetStringFromASCII(b, 12);
        fixed (byte* b = postal) result.Postal = ASCIIStringUtils.GetStringFromASCII(b, 12);
        fixed (byte* b = city) result.City = ASCIIStringUtils.GetStringFromASCII(b, 24);
        fixed (byte* b = organization) result.Organization = ASCIIStringUtils.GetStringFromASCII(b, 32);
        result.Latitude = latitude;
        result.Longitude = longitude;

        return result;
    }


    public record Managed
    {
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? Postal { get; set; }
        public string? City { get; set; }
        public string? Organization { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}
