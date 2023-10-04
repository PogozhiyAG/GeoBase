using System.Net;

namespace GeoIp.Utils;

public static class IPAddressExtensions
{
    public static uint ToInteger(this IPAddress ipAddress)
    {
        var bytes = ipAddress.GetAddressBytes();

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return BitConverter.ToUInt32(bytes, 0);
    }
}
