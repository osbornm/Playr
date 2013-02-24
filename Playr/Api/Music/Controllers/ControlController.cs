using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Playr.Api.Control.Controllers
{
    [Authorize]
    public class ControlController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Pause()
        {
            ControlNotNull();
            Program.control.Pause();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage Resume()
        {
            ControlNotNull();
            Program.control.Resume();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage Next()
        {
            ControlNotNull();
            Program.control.Next();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage Previous()
        {
            ControlNotNull();
            Program.control.Previous();
            return Request.CreateResponse(HttpStatusCode.OK);
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
