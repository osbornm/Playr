using System.Web.Http;
using Playr.Services;

namespace Playr.Api
{
    public class MusicLibraryControllerBase : ApiController
    {
        public MusicLibraryControllerBase(string routeName)
        {
            MusicLibraryService = new MusicLibraryService();
            RouteName = routeName;
        }

        public MusicLibraryService MusicLibraryService { get; set; }

        public string RouteName { get; set; }

        [NonAction]
        public string SelfLink(string ravenDbIdentifier)
        {
            return Url.SelfLink(RouteName, ravenDbIdentifier);
        }
    }
}