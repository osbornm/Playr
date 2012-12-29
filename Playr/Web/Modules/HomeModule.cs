using Nancy;

namespace Playr.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = Index;
        }

        public dynamic Index(dynamic parameters)
        {
            return View["index"];
        }
    }
}
