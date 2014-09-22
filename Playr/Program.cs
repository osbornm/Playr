using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Playr.Api.Music.Models;
using Playr.Hubs;
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
                Log.Error((Exception)args.ExceptionObject);
                Environment.Exit(-1);
            };

            try
            {
                var baseUrl = ConfigurationManager.AppSettings["Playr:Url"] ?? "http://localhost:5555/";
                FanartApiKey = ConfigurationManager.AppSettings["Playr:FanartApiKey"];

                // TODO: Remove me, this should be some kind of setting
                var exePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                TempPath = Path.Combine(exePath, "Temp");
                MusicLibraryPath = Path.Combine(exePath, "Music");
                AlbumArtworkPath = Path.Combine(exePath, "Artwork", "Album");
                FanArtworkPath = Path.Combine(exePath, "Artwork", "Fan");

                PathHelpers.EnsurePathExists(TempPath, forceClean: true);
                PathHelpers.EnsurePathExists(MusicLibraryPath);
                PathHelpers.EnsurePathExists(AlbumArtworkPath);
                PathHelpers.EnsurePathExists(FanArtworkPath);

                // Setup SignalR JSON serialization to use lowercase
                var settings = new JsonSerializerSettings();
                settings.ContractResolver = new SignalRContractResolver();
                settings.Converters.Add(new StringEnumConverter());
                var serializer = JsonSerializer.Create(settings);

                GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);

                // First Run stuff...
                var library = new MusicLibraryService();
                var trackCount = library.TotalTrackCount();
                if (trackCount < 1)
                {
                    foreach (var file in Directory.GetFiles(MusicLibraryPath, "*.*", SearchOption.AllDirectories))
                    {
                        library.ProcessFile(file);
                    }

                    foreach (var folder in Directory.GetDirectories(MusicLibraryPath, "*.*", SearchOption.AllDirectories).Reverse())
                    {
                        try
                        {
                            Directory.Delete(folder);
                        }
                        catch { }
                    }

                    trackCount = library.TotalTrackCount();
                    if (trackCount < 1)
                    {
                        Console.WriteLine("Add music to '{0}'", MusicLibraryPath);
                        return;
                    }
                }

                using (WebApp.Start<Startup>(baseUrl))
                using (var audio = new AudioService())
                using (control = new ControlService(audio))
                {
                    control.CurrentTrackChanged += track =>
                    {
                        Console.WriteLine(track.Name);
                        var current = new CurrentTrack(control.CurrentAlbum, control.CurrentTrack, control.CurrentTime.TotalMilliseconds, control.AudioState);
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                        context.Clients.All.CurrentTrackChanged(current);
                    };
                    control.Paused += () =>
                    {
                        Console.WriteLine("Paused Playing");
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                        context.Clients.All.StateChanged(audio.AudioState);
                    };
                    control.Resumed += () =>
                    {
                        Console.WriteLine("Resumed Playing");
                        var current = new CurrentTrack(control.CurrentAlbum, control.CurrentTrack, control.CurrentTime.TotalMilliseconds, control.AudioState);
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                        context.Clients.All.StateChanged(audio.AudioState, current);
                    };
                    control.Disposed += () =>
                    {
                        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                        context.Clients.All.Shutdown();
                    };

                    Console.WriteLine("There are {0} tracks in your library", trackCount);
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
                Log.Error(ex);
            }
        }
    }
}
