using System.Reflection;
using System.Web.Http;
using Owin;
using Playr.Api;

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
#if DEBUG
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
#endif
            ApiRoutes.Register(config.Routes);
            builder.UseHttpServer(config);

            // Nancy
            Assembly.Load("Nancy.ViewEngines.Razor");
            builder.UseNancy();
        }
    }
}