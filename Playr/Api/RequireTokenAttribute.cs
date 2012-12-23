using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Playr.DataModels;

namespace Playr.Api
{
    // TODO: should probably also consider supporting a querystring param... 
    public class RequireTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            var header = context.Request.Headers.SingleOrDefault(x => String.Equals(x.Key, "Playr-Token", StringComparison.InvariantCultureIgnoreCase));

            if (header.Value == null)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Must provide a valid token.");
                return;
            }

            using (var session = Database.OpenSession())
            {
                if (!session.Query<DbUser>().Any(u => u.ApiToken == header.Value.First()))
                {
                    context.Response = context.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Must provide a valid token.");
                    return;
                }
            }
        }
    }
}
