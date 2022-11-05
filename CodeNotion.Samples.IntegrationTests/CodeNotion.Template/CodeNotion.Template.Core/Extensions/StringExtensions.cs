using System;
using System.IO;
using System.Text;

namespace CodeNotion.Template.Business.Extensions;

public static class StringExtensions
{
    public static string Remove(this string source, string toBeRemoved) =>
        source.Replace(toBeRemoved, string.Empty);

    public static MemoryStream AsStream(this string source)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(source);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    public static byte[] AsByteArray(this string source) =>
        Encoding.ASCII.GetBytes(source);

    public static string AsBase64(this string source) =>
        Convert.ToBase64String(source.AsByteArray());

    public static string ToLowerFirstChar(this string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        return char.ToLower(s[0]) + s.Substring(1);
    }

    public static string Truncate(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }
}