using System;
using System.IO;
using System.Reflection;
using Microsoft.Owin.Hosting;
using Playr.DataModels;

namespace Playr
{
    public class Program
    {
        public static string MusicLibraryPath { get; set; }
        public static string TempPath { get; set; }

        static void Main(string[] args)
        {
            var baseUrl = "http://localhost:5555/";

            var exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            Database.Initialize();

            TempPath = EnsurePathExists(exePath, "Temp", clean: true);
            MusicLibraryPath = EnsurePathExists(exePath, "Music");

            using (WebApplication.Start<Startup>(baseUrl, "Microsoft.Owin.Host.HttpListener"))
            {
                Console.WriteLine("Playr is running at {0}", baseUrl);
                Console.WriteLine("See http://github.com/osbornm/playr for more information on setup.");
                Console.WriteLine();
                Console.WriteLine("Press any key to stop server...");
                Console.ReadKey(intercept: true);
            }
        }

        public static void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static string EnsurePathExists(string basePath, string subFolder, bool clean = false)
        {
            string result = Path.Combine(basePath, subFolder);

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }
            else if (clean)
            {
                Directory.Delete(result, recursive: true);
                Directory.CreateDirectory(result);
            }

            return result;
        }
    }
}
