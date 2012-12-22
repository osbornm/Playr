using System.Reflection;
using System.Web.Http;
using Owin;

namespace Playr
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            // SignalR
            builder.MapHubs("/signalr");

            // Web API
            var config = new HttpConfiguration();
            Routes.RegisterRoutes(config.Routes);
            builder.UseHttpServer(config);

            // Nancy
            Assembly.Load("Nancy.ViewEngines.Razor");
            builder.UseNancy();
        }
    }
}