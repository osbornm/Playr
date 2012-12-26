using System.Web.Http.Routing;

namespace Playr.Api.Models
{
    public class Genre
    {
        public Genre() { }

        public Genre(string name, UrlHelper url)
        {
            _Albums = url.LinkToAlbumsByGenre(name);

            Name = name;
        }

        public string _Albums { get; set; }

        public string Name { get; set; }
    }
}