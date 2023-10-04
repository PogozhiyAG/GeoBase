using System.Runtime.InteropServices;

namespace GeoIp.Data;

/// <summary>
///  Range of IP addresses
/// </summary>
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public unsafe struct IpAddressRange
{
    [FieldOffset(0)] public uint From;                       // Start of range
    [FieldOffset(4)] public uint To;                         // End of range
    [FieldOffset(8)] public uint LocationIndex;              // Index of the location record
}
