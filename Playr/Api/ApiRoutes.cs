using System.Web.Http;

namespace Playr.Api
{
    public static class ApiRoutes
    {
        public static void Register(HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "Library",
                routeTemplate: "api/library",
                defaults: new { controller = "Library" }
            );

            routes.MapHttpRoute(
                name: "Library-Albums",
                routeTemplate: "api/library/albums/{id}",
                defaults: new { controller = "Albums", id = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "Library-Tracks",
                routeTemplate: "api/library/tracks/{id}",
                defaults: new { controller = "Tracks", id = RouteParameter.Optional }
            );
        }
    }
}