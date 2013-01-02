using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gate;
using Playr.Services;

namespace Playr.Owin
{
    public class PlayrAuthMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> next;
        private readonly UserService userService = new UserService();

        public PlayrAuthMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            this.next = next;
        }

        private void Authenticate(Request request)
        {
            // Start with query string...
            string queryValue;
            if (request.Query.TryGetValue(AuthHelpers.AuthQueryStringName, out queryValue) && !String.IsNullOrWhiteSpace(queryValue))
            {
                var user = userService.GetUserByApiToken(queryValue);
                if (user != null)
                {
                    AuthHelpers.CurrentUser = new PlayrIdentity(user);
                    return;
                }
            }

            // ...then headers...
            string[] headerValues;
            if (request.Headers.TryGetValue(AuthHelpers.AuthHeaderName, out headerValues))
            {
                string headerValue = headerValues.FirstOrDefault();
                if (!String.IsNullOrWhiteSpace(headerValue))
                {
                    var user = userService.GetUserByApiToken(headerValue);
                    if (user != null)
                    {
                        AuthHelpers.CurrentUser = new PlayrIdentity(user);
                        return;
                    }
                }
            }

            // ...and finally, an authentication cookie
            string cookieValue;
            if (request.Cookies.TryGetValue(AuthHelpers.AuthCookieName, out cookieValue))
            {
                PlayrIdentity user = AuthHelpers.Decode(cookieValue);
                if (user != null)
                    AuthHelpers.CurrentUser = user;
            }
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            Authenticate(new Request(env));
            return next(env);
        }
    }
}