using System.Net;
using System.Net.Http;
using System.Web.Http;
using Playr.Api.Library.Models;

namespace Playr.Api.Library.Controllers
{
    public class TracksController : MusicLibraryControllerBase
    {
        public Track GetTrack(int id)
        {
            var track = MusicLibraryService.GetTrackById(id);
            if (track == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return new Track(track);
        }

        [HttpPost]
        public Track PostQueueOnly(int id)
        {
            var updatedTrack = MusicLibraryService.QueueOnly(id);
            if (updatedTrack == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return new Track(updatedTrack);
        }
    }
}
