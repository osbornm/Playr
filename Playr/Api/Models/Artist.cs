using System.Web.Http.Routing;

namespace Playr.Api.Models
{
    public class Artist : ModelWithLinks
    {
        public Artist(string name, UrlHelper url)
        {
            AddLink("albums", url.LinkToAlbumsByArtist(name));
            AddLink("download", url.LinkToArtistDownload(name));

            Name = name;
        }

        public string Name { get; set; }
    }
}