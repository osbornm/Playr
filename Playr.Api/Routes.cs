using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Playr.Api
{
    public static class Routes
    {
        public static void RegisterRoutes(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "playr/pause",
                routeTemplate: "pause",
                defaults: new { controller = "control", action = "pause" }
            );

            routes.MapHttpRoute(
                name: "playr/play",
                routeTemplate: "play",
                defaults: new { controller = "control", action = "play" }
            );

            routes.MapHttpRoute(
                name: "playr/next",
                routeTemplate: "next",
                defaults: new { controller = "control", action = "next" }
            );

            routes.MapHttpRoute(
                name: "playr/previous",
                routeTemplate: "previous",
                defaults: new { controller = "control", action = "previous" }
            );

            routes.MapHttpRoute(
                name: "playr/volume/up",
                routeTemplate: "volume/up",
                defaults: new { controller = "control", action = "VolumeUp" }
            );

            routes.MapHttpRoute(
                name: "playr/volume/down",
                routeTemplate: "volume/down",
                defaults: new { controller = "control", action = "VolumeDown" }
            );

            routes.MapHttpRoute(
                name: "playr/current",
                routeTemplate: "current",
                defaults: new { controller = "info", action = "CurrentTrack" }
            );

            routes.MapHttpRoute(
                name: "playr/queue",
                routeTemplate: "queue",
                defaults: new { controller = "info", action = "Queue" }
            );

            routes.MapHttpRoute(
                name: "playr/queue/id",
                routeTemplate: "queue/{id}",
                defaults: new { controller = "info", action = "QueueSong" }
            );

            routes.MapHttpRoute(
                name: "playr/songs/id/download",
                routeTemplate: "songs/{id}/download",
                defaults: new { controller = "info", action = "DownloadSong" }
            );

            routes.MapHttpRoute(
                name: "playr/songs/id/artwork",
                routeTemplate: "songs/{id}/artwork",
                defaults: new { controller = "info", action = "Artwork" }
            );

            routes.MapHttpRoute(
                name: "playr/songs/id/favorite",
                routeTemplate: "songs/{id}/favorite",
                defaults: new { controller = "info", action = "favorite" }
            );

            routes.MapHttpRoute(
                name: "playr/albums/name/download",
                routeTemplate: "albums/{name}/download",
                defaults: new { controller = "info", action = "DownloadAlbum" }
            );

            routes.MapHttpRoute(
                name: "playr/upload",
                routeTemplate: "upload",
                defaults: new { controller = "info", action = "upload" }
            );

            routes.MapHttpRoute(
                name: "playr/say",
                routeTemplate: "say",
                defaults: new { controller = "info", action = "speak" }
            );

            routes.MapHttpRoute(
                name: "playr/users/register",
                routeTemplate: "users/register",
                defaults: new { controller = "users", action = "register" }
            );

            routes.MapHttpRoute(
                name: "playr/users/email",
                routeTemplate: "users/{email}",
                defaults: new { controller = "users", action = "find" }
            );

            routes.MapHttpRoute(
                name: "playr/users/email/reset",
                routeTemplate: "users/{email}/reset",
                defaults: new { controller = "users", action = "resetToken" }
            );

            routes.MapHttpRoute("default", "{controller}/{action}");
        }
    }
}
