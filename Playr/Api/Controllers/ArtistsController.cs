using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Playr.Api.Models;

namespace Playr.Api
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