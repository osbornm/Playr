using System.Web.Http.Routing;

namespace Playr.Api.Models
{
    public class Artist
    {
        public Artist(string name, UrlHelper url)
        {
            _Albums = url.LinkToAlbumsByArtist(name);
            _Download = url.LinkToArtistDownload(name);

            Name = name;
        }

        public string _Albums { get; set; }
        public string _Download { get; set; }

        public string Name { get; set; }
    }
}