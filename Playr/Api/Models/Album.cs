using System.Web.Http.Routing;
using Playr.DataModels;

namespace Playr.Api.Models
{
    public class Album : ModelWithLinks
    {
        public Album() { }

        public Album(DbAlbum album, UrlHelper url)
        {
            AddLink("self", url.LinkToAlbum(album));
            AddLink("tracks", url.LinkToTracks(album));
            AddLink("download", url.LinkToAlbumDownload(album));

            ArtistName = album.ArtistName;
            Genre = album.Genre;
            Name = album.Name;
        }

        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
    }
}