using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.SelfHost;
using System.Web.Http;
using System.Net.Http;
using iTunesLib;
using System.Runtime.InteropServices;

namespace Playr.Api
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:5555");

            config.Routes.MapHttpRoute(
                name: "artwork",
                routeTemplate: "songs/Artwork/{id}",
                defaults: new { controller = "info", action = "artwork"}
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
    }
}
