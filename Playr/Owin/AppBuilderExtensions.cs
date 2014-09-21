using Owin;
using Playr.Owin;

public static class AppBuilderExtensions
{
    //public static IAppBuilder UseNancy(this IAppBuilder app)
    //{
    //    return app.Use(typeof(NancyHandler));
    //}

    //public static IAppBuilder UseNancy(this IAppBuilder app, INancyBootstrapper bootstrapper)
    //{
    //    return app.Use(typeof(NancyHandler), bootstrapper);
    //}

    public static IAppBuilder UsePlayrAuthentication(this IAppBuilder app)
    {
        return app.Use(typeof(PlayrAuthMiddleware));
    }
}