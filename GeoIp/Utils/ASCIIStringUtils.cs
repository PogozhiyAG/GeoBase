using System.Text;

namespace GeoIp.Utils;

public static class ASCIIStringUtils
{
    private static ASCIIEncoding ascii = new ASCIIEncoding();

    public unsafe static string GetStringFromASCII(byte* ptr, int maxCount)
    {
        int count = 0;

        while (count < maxCount && *(ptr + count) != 0)
        {
            count++;
        }

        return new string((sbyte*)ptr, 0, count);
    }


    public static byte[] GetASCIIFromString(string s)
    {
        byte[] buffer = new byte[s.Length + 1]; // with a terminal zero
        ascii.GetBytes(s, 0, s.Length, buffer, 0);
        return buffer;
    }

    public unsafe static int CompareASCIIStrings(byte* array1, byte* array2, int count)
    {
        int cmp = 0;

        for (; cmp == 0 && count-- > 0; array1++, array2++)
        {
            cmp = array1->CompareTo(*array2);
            if (*array1 == 0 || *array2 == 0)
                break;
        }

        return cmp;
    }
}
