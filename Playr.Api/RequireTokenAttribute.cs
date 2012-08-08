using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Playr.Api.Models;
using Raven.Client.Embedded;
using System.Net.Http;

namespace Playr.Api
{
    // TODO: should probably also consider supporting a querystring param... 
    public class RequireTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var header = context.Request.Headers.SingleOrDefault(x => x.Key == "x-playr-token");

            if (header.Value == null)
            {
                throw new HttpResponseException(context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Must provide a valid token."));
            }

            using (var session = Helpers.DocumentStore.OpenSession())
            {
                if (!session.Query<User>().Where(u => u.Token == header.Value.First()).Any())
                {
                    throw new HttpResponseException(context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Must provide a valid token."));
                }
            }
        }
    }
}
