using System.Net.Http.Headers;
using Microsoft.Win32;

public static class MediaTypeHeaderValueExtensions
{
    public static bool IsAudio(this MediaTypeHeaderValue mediaType)
    {
        return mediaType.MediaType.StartsWith("audio/");
    }

    public static bool IsMultipartFormData(this MediaTypeHeaderValue mediaType)
    {
        return mediaType.MediaType == "multipart/form-data";
    }

    public static bool IsZipFile(this MediaTypeHeaderValue mediaType)
    {
        return mediaType.MediaType == "application/x-zip-compressed";
    }

    public static string ToFileExtension(this MediaTypeHeaderValue mediaType)
    {
        string result = null;

        using (var key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mediaType.MediaType, writable: false))
        {
            if (key != null)
                result = key.GetValue("Extension") as string;
        }

        return result ?? "." + mediaType.MediaType.Substring(mediaType.MediaType.IndexOf('/') + 1);
    }
}