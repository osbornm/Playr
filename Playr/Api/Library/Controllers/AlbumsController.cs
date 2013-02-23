using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Playr.Api.Library.Models;

namespace Playr.Api.Library.Controllers
{
    public class AlbumsController : MusicLibraryControllerBase
    {
        [HttpGet]
        public IEnumerable<Album> Albums()
        {
            return MusicLibraryService.GetAlbums()
                                      .Select(dbAlbum => new Album(dbAlbum, Url))
                                      .ToList();
        }

        [HttpGet]
        public Album Album(int id)
        {
            var dbAlbum = MusicLibraryService.GetAlbumById(id);
            if (dbAlbum == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return new Album(dbAlbum, Url);
        }

        [HttpGet]
        public Album Artwork(int id)
        {
            return null;
        }
    }
}