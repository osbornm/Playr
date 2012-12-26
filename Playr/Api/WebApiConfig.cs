using System.Web.Http;

namespace Playr.Api
{
    public static class WebApiConfig
    {
        public static class Routes
        {
            public const string Albums = "Library-Albums";
            public const string Genres = "Library-Genres";
            public const string Library = "Library";
            public const string Tracks = "Library-Tracks";
        }

        public static void Configure(HttpConfiguration config)
        {
#if DEBUG
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
#endif

            config.Routes.MapHttpRoute(
                name: Routes.Library,
                routeTemplate: "api/library",
                defaults: new { controller = "Library" }
            );

            config.Routes.MapHttpRoute(
                name: Routes.Genres,
                routeTemplate: "api/library/genres/{id}",
                defaults: new { controller = "Genres", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: Routes.Albums,
                routeTemplate: "api/library/albums/{id}",
                defaults: new { controller = "Albums", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: Routes.Tracks,
                routeTemplate: "api/library/albums/{id}/tracks",
                defaults: new { controller = "Tracks" }
            );
        }
    }
}