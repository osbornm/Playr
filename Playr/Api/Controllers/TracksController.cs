using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Playr.Api
{
    public class TracksController : MusicLibraryControllerBase
    {
        public TracksController()
            : base("Library-Tracks") { }

        public IEnumerable<Track> GetTracks()
        {
            return MusicLibraryService.GetTracks()
                                      .Select(dbTrack => new Track(dbTrack, SelfLink(dbTrack.Id)));
        }

        public Track GetTrackById(int id)
        {
            var dbTrack = MusicLibraryService.GetTrackById(id);
            if (dbTrack == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return new Track(dbTrack, SelfLink(dbTrack.Id));
        }
    }
}
