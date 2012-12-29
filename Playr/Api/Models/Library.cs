using System.Web.Http.Routing;
using Playr.DataModels;

namespace Playr.Api.Models
{
    public class Library
    {
        public Library(DbLibrary library, UrlHelper url)
        {
            _Albums = url.LinkToAlbums();
            _Artists = url.LinkToArtists();
            _Genres = url.LinkToGenres();
            _Upload = url.LinkToLibrary();

            TotalAlbums = library.TotalAlbums;
            TotalTracks = library.TotalTracks;
        }

        public string _Albums { get; set; }
        public string _Artists { get; set; }
        public string _Genres { get; set; }
        public string _Upload { get; set; }

        public int TotalAlbums { get; set; }
        public int TotalTracks { get; set; }
    }
}
