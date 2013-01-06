using System.Collections.Generic;
using System.Linq;
using Playr.Api.Library.Models;

namespace Playr.Api.Library.Controllers
{
    public class ArtistsController : MusicLibraryControllerBase
    {
        public List<Artist> GetArtists()
        {
            return MusicLibraryService.GetArtists()
                                      .Select(artist => new Artist(artist, Url))
                                      .ToList();
        }

        public List<Album> GetAlbumsByArtist(string artistName)
        {
            return MusicLibraryService.GetAlbumsByArtist(artistName)
                                      .Select(album => new Album(album, Url))
                                      .ToList();
        }
    }
}