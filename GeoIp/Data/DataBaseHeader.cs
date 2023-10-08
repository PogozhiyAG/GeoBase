using System.Runtime.InteropServices;

namespace GeoIp.Data;

/// <summary>
/// Database header structure
/// </summary>
[StructLayout(LayoutKind.Explicit, Pack = 1)]
public unsafe struct DataBaseHeader
{
    [FieldOffset(0)] public int Version;              // database version
    [FieldOffset(4)] public fixed byte Name[32];      // name of the database
    [FieldOffset(36)] public ulong Timestamp;         // creation time
    [FieldOffset(44)] public int Records;             // number of records
    [FieldOffset(48)] public uint OffsetRanges;       // offset from the beginning of the file to the beginning of the list of records with geoinformation
    [FieldOffset(52)] public uint OffsetCities;       // offset from the beginning of the file to the beginning of the index, sorted by city name
    [FieldOffset(56)] public uint OffsetLocations;    // offset from the start of the file to the start of the list of location records
}
