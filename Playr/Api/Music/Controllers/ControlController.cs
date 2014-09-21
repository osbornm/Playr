using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Playr.Api.Control.Controllers
{
    // TODO: uncomment once we have auth working 
    //[Authorize]
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
