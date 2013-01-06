using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public static class HttpContentExtensions
{
    public static async Task ReadAsFileAsync(this HttpContent content, string filename)
    {
        using (var stream = new FileStream(Path.GetFullPath(filename), FileMode.Create, FileAccess.Write, FileShare.None))
            await content.CopyToAsync(stream);
    }
}