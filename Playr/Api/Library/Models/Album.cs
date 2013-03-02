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
            AddLink("artwork", LibraryEndpoints.LinkToAlbumArt(album));
            AddLink("self", LibraryEndpoints.LinkToAlbum(album));
            AddLink("tracks", url.LinkToTracks(album));
            AddLink("download", LibraryEndpoints.LinkToAlbumDownload(album));

            ArtistName = album.ArtistName;
            Genre = album.Genre;
            Name = album.Name;
        }

        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
    }
}