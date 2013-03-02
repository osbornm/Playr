using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Playr.Api.Library.Models;

namespace Playr.Api.Library.Controllers
{
    public class TracksController : MusicLibraryControllerBase
    {
        public IEnumerable<Track> GetTracks(int id)
        {
            var tracks = MusicLibraryService.GetTracks(id);
            if (tracks == null || tracks.Count == 0)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return tracks.Select(dbTrack => new Track(dbTrack));
        }
    }
}
