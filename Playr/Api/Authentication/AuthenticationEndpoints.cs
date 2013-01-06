using System.Web.Http;

public static class AuthenticationEndpoints
{
    const string Login = "Auth-Login";
    const string Logout = "Auth-Logout";
    const string Registration = "Auth-Registration";

    public static void Configure(HttpConfiguration config)
    {
        config.Routes.MapHttpRoute(
            name: Login,
            routeTemplate: "api/auth/login",
            defaults: new { controller = "Authentication", action = "Login" }
        );

        config.Routes.MapHttpRoute(
            name: Logout,
            routeTemplate: "api/auth/logout",
            defaults: new { controller = "Authentication", action = "Logout" }
        );

        config.Routes.MapHttpRoute(
            name: Registration,
            routeTemplate: "api/auth/register",
            defaults: new { controller = "Authentication", action = "Register" }
        );
    }
}