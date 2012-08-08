using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.SelfHost;
using System.Web.Http;
using System.Net.Http;
using iTunesLib;
using System.Runtime.InteropServices;
using System.IO;
using Playr.Api.Handelrs;
using SignalR.Hosting.Self;
using SignalR;
using System.Web.Http.Routing;
using Playr.Api.Models;

namespace Playr.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration(ApplicationSettings.apiBaseUrl);
            config.MaxReceivedMessageSize = 1024 * 1024 * 1024;
            config.MessageHandlers.Add(new CorsHandler());
            Routes.RegisterRoutes(config.Routes);


            var apiServer = new HttpSelfHostServer(config);
            apiServer.OpenAsync().Wait();
            var signalrServer = new Server(ApplicationSettings.signalrBaseUrl);
            signalrServer.MapHubs("/signalr");
            signalrServer.Start();

            Helpers.InitializeDocumentStore();
            var itunes = new iTunesAppClass();
            Start(itunes);

            Console.WriteLine("Press any key to stop server...");
            Console.ReadLine();

            // Stop Everything
            apiServer.CloseAsync().Wait();
            signalrServer.Stop();
            Stop(itunes);
            Marshal.ReleaseComObject(itunes);
        }

        public static void Start(iTunesAppClass itunes)
        {
            SetUp(itunes);
            if (itunes.CurrentPlaylist == null || itunes.CurrentPlaylist.Name != "iTunes DJ")
            {
                var playlist = itunes.LibrarySource.Playlists.get_ItemByName("iTunes DJ");
                if (playlist != null)
                {
                    playlist.PlayFirstTrack();
                }
                else
                {
                    throw new Exception("iTunes DJ playlist not found.");
                }
            }
        }

        public static void Stop(iTunesAppClass itunes)
        {
            itunes.Stop();
            itunes.Quit();

            Directory.Delete(ApplicationSettings.TempPath, true);
        }

        public static void SetUp(iTunesAppClass itunes)
        {
            // Check for artwork folder
            var artwork = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Artwork\\");
            if (!Directory.Exists(artwork))
            {
                Directory.CreateDirectory(artwork);
            }
            ApplicationSettings.ArtworkFolder = artwork;

            // Check for temp folder
            var temp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp\\");
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            ApplicationSettings.TempPath = temp;

            // Automatically Add to Itunes folder
            // TODO: Is this Okay to do? Do i want to assume this is set up? Add to readme?
            ApplicationSettings.iTunesAddFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "iTunes\\iTunes Media\\Automatically Add to iTunes");

            // Set up track change
            itunes.OnPlayerPlayEvent += itunes_OnPlayerPlayEvent;

        }

        static void itunes_OnPlayerPlayEvent(object iTrack)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<Playr.Api.Hubs.PlayrHub>();
            hub.Clients.DjInfoUpdated();
        }

    }
}
