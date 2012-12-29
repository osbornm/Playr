using System.Web.Http.Routing;
using Playr.DataModels;

namespace Playr.Api
{
    public class Album
    {
        public Album() { }

        public Album(DbAlbum album, UrlHelper url)
        {
            _Download = url.LinkToAlbumDownload(album);
            _Self = url.LinkToAlbum(album);
            _Tracks = url.LinkToTracks(album);

            ArtistName = album.ArtistName;
            Genre = album.Genre;
            Name = album.Name;
        }

        public string _Download { get; set; }
        public string _Self { get; set; }
        public string _Tracks { get; set; }

        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
    }
}