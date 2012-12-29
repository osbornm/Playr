using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Owin.Hosting;

namespace Playr
{
    public class Program
    {
        public static string MusicLibraryPath { get; set; }
        public static string TempPath { get; set; }

        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Console.WriteLine(args.ExceptionObject);
                Environment.Exit(-1);
            };

            try
            {
                var baseUrl = ConfigurationManager.AppSettings["Playr:Url"] ?? "http://localhost:5555/";

                // TODO: Remove me
                var exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                TempPath = Path.Combine(exePath, "Temp");
                MusicLibraryPath = Path.Combine(exePath, "Music");

                PathHelpers.EnsurePathExists(TempPath, forceClean: true);
                PathHelpers.EnsurePathExists(MusicLibraryPath);

                using (WebApplication.Start<Startup>(baseUrl, "Microsoft.Owin.Host.HttpListener"))
                {
                    Console.WriteLine("Playr is running at {0}", baseUrl);
                    Console.WriteLine("See http://github.com/osbornm/playr for more information on setup.");
                    Console.WriteLine();
                    Console.WriteLine("Press any key to stop server...");
                    Console.ReadKey(intercept: true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
