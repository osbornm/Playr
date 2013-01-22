using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Playr.Api.Library.Models;

namespace Playr.Api.Music.Controllers
{
    // api/info/currentTrack
    // api/info/queue

    public class InfoController : ApiController
    {
        [HttpGet]
        public Track CurrentTrack()
        {
            ControlNotNull();
            return new Track(Program.control.CurrentTrack, Url);
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
