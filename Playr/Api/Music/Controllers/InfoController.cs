using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Playr.Api.Music.Models;

namespace Playr.Api.Music.Controllers
{
    public class InfoController : ApiController
    {
        [HttpGet]
        public CurrentTrack Current()
        {
            ControlNotNull();
            return new CurrentTrack(Program.control.CurrentAlbum, Program.control.CurrentTrack, Program.control.CurrentTime.TotalMilliseconds, Url);
        }

        [HttpGet]
        public CurrentTrack Queue()
        {
            throw new NotImplementedException();
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
