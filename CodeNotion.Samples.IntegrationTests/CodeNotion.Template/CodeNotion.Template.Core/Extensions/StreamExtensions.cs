using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeNotion.Template.Business.Extensions;

public static class StreamExtensions
{
    public static MemoryStream Reset(this MemoryStream source)
    {
        source.Seek(0, SeekOrigin.Begin);
        return source;
    }
        
    public static Stream Reset(this Stream source)
    {
        source.Seek(0, SeekOrigin.Begin);
        return source;
    }

    public static async Task<string> ReadTextAsync(this Stream source, Encoding? encoding = null)
    {
        using var reader = new StreamReader(source, encoding ?? Encoding.Default);
        var result = new char[reader.BaseStream.Length];
        await reader.ReadAsync(result, 0, (int)reader.BaseStream.Length);
        return new string(result);
    }
}