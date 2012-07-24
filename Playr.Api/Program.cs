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

namespace Playr.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:5555");
            config.MaxReceivedMessageSize = 1024 * 1024 * 1024;

            config.Routes.MapHttpRoute(
                name: "artwork",
                routeTemplate: "songs/Artwork/{id}",
                defaults: new { controller = "info", action = "artwork" }
            );


            config.Routes.MapHttpRoute("default", "{controller}/{action}");



            var server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
            Helpers.InitializeDocumentStore();
            var itunes = new iTunesAppClass();
            Start(itunes);

            Console.WriteLine("Press any key to stop server...");
            Console.ReadLine();

            // Stop Everything
            server.CloseAsync().Wait();
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

            // Check for upload folder
            var upload = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload\\");
            if (!Directory.Exists(upload))
            {
                Directory.CreateDirectory(upload);
            }
            ApplicationSettings.UploadPath = upload;

            // Automatically Add to Itunes folder
            // TODO: Is this Okay to do? Do i want to assume this is set up? Add to readme?
            ApplicationSettings.iTunesAddFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "iTunes\\iTunes Media\\Automatically Add to iTunes");

        }

    }
}
