using System.Web.Http;

namespace Playr
{
    public static class Routes
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "GetAllAlbums",
                routeTemplate: "api/library/albums",
                defaults: new { controller = "MusicLibrary", action = "Albums" }
            );

            routes.MapHttpRoute(
                name: "GetAllTracks",
                routeTemplate: "api/library/tracks",
                defaults: new { controller = "MusicLibrary", action = "Tracks" }
            );

            routes.MapHttpRoute(
                name: "AddToLibrary",
                routeTemplate: "api/library",
                defaults: new { controller = "MusicLibrary", action = "upload" }
            );
        }
    }
}