using Nancy;

namespace Playr.Web.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = Index;

            // Stand alone fullscreen information screen
            Get["/fullscreen"] = FullScreen;
            Get["/lite"] = FullScreen;

        }

        public dynamic Index(dynamic parameters)
        {
            return View["index"];
        }

        public dynamic FullScreen(dynamic parameters)
        {
            return View["fullscreen"];
        }
    }
}
