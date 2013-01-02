using Owin;
using Playr.Api;

namespace Playr.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            builder.UsePlayrAuthentication()
                   .MapHubs("/signalr")
                   .UseHttpServer(WebApiConfig.GetConfiguration())
                   .UseNancy();
        }
    }
}