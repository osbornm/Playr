using System.Net;
using System.Net.Http;
using System.Web.Http;
using Playr.Api.Music.Models;

namespace Playr.Api.Music.Controllers
{
    // api/info/currentTrack
    // api/info/queue

    public class InfoController : ApiController
    {
        [HttpGet]
        public CurrentTrack CurrentTrack()
        {
            ControlNotNull();
            return new CurrentTrack(Program.control.CurrentAlbum, Program.control.CurrentTrack, Url);
        }

        private void ControlNotNull()
        {
            if (Program.control == null)
            {
                throw new HttpResponseException(
                    Request.CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "Error in control service, was null"));
            }
        }
    }
}
