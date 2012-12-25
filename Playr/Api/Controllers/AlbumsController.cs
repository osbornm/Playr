using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Playr.Api
{
    public class AlbumsController : MusicLibraryControllerBase
    {
        public AlbumsController()
            : base("Library-Albums") { }

        public IEnumerable<Album> GetAlbums()
        {
            return MusicLibraryService.GetAlbums()
                                      .Select(dbAlbum => new Album(dbAlbum, Link(dbAlbum)));
        }

        public Album GetAlbumById(int id)
        {
            var dbAlbum = MusicLibraryService.GetAlbumById(id);
            if (dbAlbum == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return new Album(dbAlbum, Link(dbAlbum));
        }
    }
}