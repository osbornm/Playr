using System;
using System.IO;
using System.Linq;
using System.Text;

public static class PathHelpers
{
    private static string ConvertAndTrim(string name, int maxLength, char[] invalidCharacters)
    {
        if (name == null)
            return null;

        maxLength = Math.Min(maxLength, name.Length);
        var result = new StringBuilder(maxLength);
        for (int idx = 0; idx < maxLength; idx++)
        {
            char c = name[idx];
            result.Append(invalidCharacters.Contains(c) ? '_' : c);
        }

        if (result.Length > 0 && result[result.Length - 1] == '.')
            result[result.Length - 1] = '_';

        return result.ToString();
    }

    public static void EnsurePathExists(string path, bool forceClean = false)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        else if (forceClean)
        {
            Directory.Delete(path, recursive: true);
            Directory.CreateDirectory(path);
        }
    }

    public static string ToFileName(string fileName, int maxLength = 40)
    {
        return ConvertAndTrim(fileName, maxLength, Path.GetInvalidFileNameChars());
    }

    public static string ToFolderName(string folderName, int maxLength = 40)
    {
        return ConvertAndTrim(folderName, maxLength, Path.GetInvalidFileNameChars());
    }
}