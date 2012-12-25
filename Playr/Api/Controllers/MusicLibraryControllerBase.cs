using System.Collections.Generic;
using System.Web.Http;
using Playr.DataModels;
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
        public string Link(int? id = null, string routeName = null)
        {
            var routeValues = new Dictionary<string, object>();
            if (id.HasValue)
                routeValues.Add("id", id.Value);

            return Url.Link(routeName ?? RouteName, routeValues);
        }

        [NonAction]
        public string Link(DbModel model, string routeName = null)
        {
            return Link(model.Id, routeName);
        }
    }
}