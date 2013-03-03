using System.Web.Http.Routing;
using Playr.Api.Shared.Models;
using Playr.DataModels;

namespace Playr.Api.Library.Models
{
    public class Album : ModelWithLinks
    {
        public Album() { }

        public Album(DbAlbum album, UrlHelper url)
        {
            AddLink("artwork", RouteLinks.LinkToAlbumArt(album));
            AddLink("self", RouteLinks.LinkToAlbum(album));
            AddLink("tracks", RouteLinks.LinkToTracks(album));
            AddLink("download", RouteLinks.LinkToAlbumDownload(album));

            ArtistName = album.ArtistName;
            Genre = album.Genre;
            Name = album.Name;
        }

        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
    }
}