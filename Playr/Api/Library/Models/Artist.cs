using System.Web.Http.Routing;
using Playr.Api.Shared.Models;

namespace Playr.Api.Library.Models
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