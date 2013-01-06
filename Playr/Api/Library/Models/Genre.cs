using System.Web.Http.Routing;
using Playr.Api.Shared.Models;

namespace Playr.Api.Library.Models
{
    public class Genre : ModelWithLinks
    {
        public Genre() { }

        public Genre(string name, UrlHelper url)
        {
            AddLink("albums", url.LinkToAlbumsByGenre(name));

            Name = name;
        }

        public string Name { get; set; }
    }
}