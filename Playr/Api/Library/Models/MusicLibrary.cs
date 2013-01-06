using System.Web.Http.Routing;
using Playr.Api.Shared.Models;
using Playr.DataModels;

namespace Playr.Api.Library.Models
{
    public class MusicLibrary : ModelWithLinks
    {
        public MusicLibrary(DbLibrary library, UrlHelper url)
        {
            AddLink("albums", url.LinkToAlbums());
            AddLink("artists", url.LinkToArtists());
            AddLink("genres", url.LinkToGenres());
            AddLink("upload", url.LinkToLibrary());

            TotalAlbums = library.TotalAlbums;
            TotalTracks = library.TotalTracks;
        }

        public int TotalAlbums { get; set; }
        public int TotalTracks { get; set; }
    }
}
