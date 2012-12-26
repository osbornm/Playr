using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Playr.Api.Models;

namespace Playr.Api.Controllers
{
    public class GenresController : MusicLibraryControllerBase
    {
        public IEnumerable<Genre> Get()
        {
            return MusicLibraryService.GetGenres()
                                      .Select(genre => new Genre(genre, Url));
        }

        public IEnumerable<Album> Get(string id)
        {
            var albums = MusicLibraryService.GetAlbumsByGenre(id);
            if (albums == null || albums.Count == 0)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return albums.Select(album => new Album(album, Url));
        }
    }
}
