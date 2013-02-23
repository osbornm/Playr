using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.Owin.Hosting;
using Playr.Owin;
using Playr.Services;

namespace Playr
{
    public class Program
    {
        public static ControlService control { get; private set; }
        public static string MusicLibraryPath { get; private set; }
        public static string AlbumArtworkPath { get; private set; }
        public static string FanArtworkPath { get; private set; }
        public static string TempPath { get; private set; }
        public static string FanartApiKey { get; set; }
        public static bool FanartEnabled { get { return !String.IsNullOrWhiteSpace(FanartApiKey); } }

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
                FanartApiKey = ConfigurationManager.AppSettings["Playr:FanartApiKey"];


                // TODO: Remove me
                var exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                TempPath = Path.Combine(exePath, "Temp");
                MusicLibraryPath = Path.Combine(exePath, "Music");
                AlbumArtworkPath = Path.Combine(exePath, "Artwork", "Album");
                FanArtworkPath = Path.Combine(exePath, "Artwork", "Fan");

                PathHelpers.EnsurePathExists(TempPath, forceClean: true);
                PathHelpers.EnsurePathExists(MusicLibraryPath);
                PathHelpers.EnsurePathExists(AlbumArtworkPath);
                PathHelpers.EnsurePathExists(FanArtworkPath);

                using (WebApplication.Start<Startup>(baseUrl, "Microsoft.Owin.Host.HttpListener"))
                using (var audio = new Playr.Services.AudioService())
                using (control = new ControlService(audio))
                {
                    control.CurrentTrackChanged += track => Console.WriteLine(track.Name);
                    control.Paused += () => Console.WriteLine("Paused Playing");
                    control.Resumed += () => Console.WriteLine("Resumed Playing");

                    Console.WriteLine("Playr is running at {0}", baseUrl);
                    Console.WriteLine("See http://github.com/osbornm/playr for more information on setup.");
                    Console.WriteLine();
                    Console.WriteLine("Press ENTER to stop server...");
                    control.Spin();
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
