using System.Web.Http;

public static class MusicEndpoints
{
    const string Control = "Control";
    const string Information = "Information";

    public static void Configure(HttpConfiguration config)
    {
        config.Routes.MapHttpRoute(
            name: Control,
            routeTemplate: "api/control/{action}",
            defaults: new { controller = "Control" }
        );

        config.Routes.MapHttpRoute(
            name: Information,
            routeTemplate: "api/info/{action}",
            defaults: new { controller = "Info" }
        );
    }
}