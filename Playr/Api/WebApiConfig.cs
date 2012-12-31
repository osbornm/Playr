using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Playr.Api
{
    public static class WebApiConfig
    {
        public static class Routes
        {
            public const string Albums = "Library-Albums";
            public const string AlbumDownload = "Library-DownloadAlbum";
            public const string Artists = "Library-Artists";
            public const string ArtistDownload = "Library-DownloadArtist";
            public const string Genres = "Library-Genres";
            public const string Library = "Library";
            public const string Tracks = "Library-Tracks";
            public const string TrackDownload = "Library-TrackDownload";
        }

        public static void Configure(HttpConfiguration config)
        {
#if DEBUG
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
#endif

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Routes.MapHttpRoute(
                name: Routes.Library,
                routeTemplate: "api/library",
                defaults: new { controller = "Library" }
            );

            config.Routes.MapHttpRoute(
                name: Routes.Albums,
                routeTemplate: "api/library/albums/{id}",
                defaults: new { controller = "Albums", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: Routes.AlbumDownload,
                routeTemplate: "api/library/albums/{id}/download",
                defaults: new { controller = "Download", action = "Album" }
            );

            config.Routes.MapHttpRoute(
                name: Routes.Artists,
                routeTemplate: "api/library/artists/{artistName}",
                defaults: new { controller = "Artists", artistName = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: Routes.ArtistDownload,
                routeTemplate: "api/library/artists/{artistName}/download",
                defaults: new { controller = "Download", action = "Artist" }
            );

            config.Routes.MapHttpRoute(
                name: Routes.Genres,
                routeTemplate: "api/library/genres/{genreName}",
                defaults: new { controller = "Genres", genreName = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: Routes.Tracks,
                routeTemplate: "api/library/albums/{id}/tracks",
                defaults: new { controller = "Tracks" }
            );

            config.Routes.MapHttpRoute(
                name: Routes.TrackDownload,
                routeTemplate: "api/library/tracks/{id}/download",
                defaults: new { controller = "Download", action = "Track" }
            );
        }
    }
}