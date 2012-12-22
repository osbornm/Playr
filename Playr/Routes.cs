using System.Web.Http;

namespace Playr
{
    public static class Routes
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "playr/pause",
                routeTemplate: "api/pause",
                defaults: new { controller = "control", action = "pause" }
            );

            routes.MapHttpRoute(
                name: "playr/play",
                routeTemplate: "api/play",
                defaults: new { controller = "control", action = "play" }
            );

            routes.MapHttpRoute(
               name: "playr/playpause",
               routeTemplate: "api/playpause",
               defaults: new { controller = "control", action = "playpause" }
           );

            routes.MapHttpRoute(
                name: "playr/next",
                routeTemplate: "api/next",
                defaults: new { controller = "control", action = "next" }
            );

            routes.MapHttpRoute(
                name: "playr/previous",
                routeTemplate: "api/previous",
                defaults: new { controller = "control", action = "previous" }
            );

            routes.MapHttpRoute(
                name: "playr/volume/up",
                routeTemplate: "api/volume/up",
                defaults: new { controller = "control", action = "VolumeUp" }
            );

            routes.MapHttpRoute(
                name: "playr/volume/down",
                routeTemplate: "api/volume/down",
                defaults: new { controller = "control", action = "VolumeDown" }
            );

            routes.MapHttpRoute(
                name: "playr/current",
                routeTemplate: "api/current",
                defaults: new { controller = "info", action = "CurrentTrack" }
            );

            routes.MapHttpRoute(
                name: "playr/queue",
                routeTemplate: "api/queue",
                defaults: new { controller = "info", action = "Queue" }
            );

            routes.MapHttpRoute(
                name: "playr/queue/id",
                routeTemplate: "api/queue/{id}",
                defaults: new { controller = "info", action = "QueueSong" }
            );

            routes.MapHttpRoute(
                name: "playr/songs/id/download",
                routeTemplate: "api/songs/{id}/download",
                defaults: new { controller = "info", action = "DownloadSong" }
            );

            routes.MapHttpRoute(
                name: "playr/songs/id/artwork",
                routeTemplate: "api/songs/{id}/artwork",
                defaults: new { controller = "info", action = "Artwork" }
            );

            routes.MapHttpRoute(
                name: "playr/songs/id/favorite",
                routeTemplate: "api/songs/{id}/favorite",
                defaults: new { controller = "info", action = "favorite" }
            );

            routes.MapHttpRoute(
                name: "playr/albums/name/download",
                routeTemplate: "api/albums/{name}/download",
                defaults: new { controller = "info", action = "DownloadAlbum" }
            );

            routes.MapHttpRoute(
                name: "playr/upload",
                routeTemplate: "api/upload",
                defaults: new { controller = "info", action = "upload" }
            );

            routes.MapHttpRoute(
                name: "playr/say",
                routeTemplate: "api/say",
                defaults: new { controller = "control", action = "speak" }
            );

            routes.MapHttpRoute(
                name: "playr/users/register",
                routeTemplate: "api/users/register",
                defaults: new { controller = "users", action = "register" }
            );

            routes.MapHttpRoute(
                name: "playr/users/email",
                routeTemplate: "api/users/{email}",
                defaults: new { controller = "users", action = "find" }
            );

            routes.MapHttpRoute(
                name: "playr/users/email/reset",
                routeTemplate: "api/users/{email}/reset",
                defaults: new { controller = "users", action = "resetToken" }
            );

            routes.MapHttpRoute(
                name: "playr/fanart/artist",
                routeTemplate: "api/artists/{artist}/fanart",
                defaults: new { controller = "fanart", action = "FanartCollection" }
            );

            routes.MapHttpRoute(
                name: "playr/fanart/artist/file",
                routeTemplate: "api/artists/{artist}/fanart/{fileName}",
                defaults: new { controller = "fanart", action = "Fanart" }
            );
        }
    }
}