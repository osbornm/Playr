using System.Web.Http;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Playr.Api
{
    public static class WebApiConfig
    {
        public static HttpConfiguration GetConfiguration()
        {
            var config = new HttpConfiguration();

#if DEBUG
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
#endif

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());

            LibraryEndpoints.Configure(config);
            AuthenticationEndpoints.Configure(config);
            MusicEndpoints.Configure(config);

            return config;
        }
    }
}