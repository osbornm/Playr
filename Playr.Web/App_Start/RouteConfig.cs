using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Playr.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "songs/{id}/favorite",
                defaults: new { controller = "Home", action = "favorite" }
            );

            routes.MapRoute(
                name: "lite",
                url: "lite",
                defaults: new { controller = "Home", action = "Lite" }
            );

            routes.MapRoute(
                name: "foo",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}