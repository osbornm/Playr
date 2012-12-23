using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public static class HttpContentExtensions
{
    public static async Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
    {
        string pathname = Path.GetFullPath(filename);
        if (!overwrite && File.Exists(filename))
        {
            throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
        }

        FileStream fileStream = null;
        try
        {
            fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
            await content.CopyToAsync(fileStream);
        }
        finally
        {
            if (fileStream != null)
            {
                fileStream.Close();
            }
        }
    }
}