using System.Web.Http.Routing;

namespace Playr.Api.Models
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