using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Playr.Api.Models;
using Playr.DataModels;
using Playr.Services;

namespace Playr.Api.Controllers
{
    public class AuthenticationController : ApiController
    {
        private readonly UserService userService = new UserService();

        public HttpResponseMessage Login(JObject body)
        {
            try
            {
                if (body != null)
                {
                    var persistent = body.Value<bool>("persistent");

                    var apiToken = body.Value<string>("apiToken");
                    if (apiToken != null)
                        return LoginWithApiToken(apiToken, persistent);

                    var emailAddress = body.Value<string>("emailAddress");
                    var password = body.Value<string>("password");
                    if (emailAddress != null && password != null)
                        return LoginWithEmailAddress(emailAddress, password, persistent);
                }
            }
            catch { }

            return Request.CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "Must provide a body with an object with either 'apiToken' or both 'emailAddress' and 'password'. Both types of requests may be accompanied with a 'persistent' value of 'true' to get a persistent cookie."
            );
        }

        private HttpResponseMessage LoginWithApiToken(string apiToken, bool persistent)
        {
            var user = userService.GetUserByApiToken(apiToken);
            if (user == null)
                return BadCredentials();

            var response = Request.CreateResponse(HttpStatusCode.OK, new User(user));
            response.Headers.AddCookies(new[] { CreateAuthCookie(user, persistent) });
            return response;
        }

        private HttpResponseMessage LoginWithEmailAddress(string emailAddress, string password, bool persistent)
        {
            var user = userService.GetUserByEmailAddress(emailAddress);
            if (user == null || !user.ValidatePassword(password))
                return BadCredentials();

            var response = Request.CreateResponse(HttpStatusCode.OK, new User(user));
            response.Headers.AddCookies(new[] { CreateAuthCookie(user, persistent) });
            return response;
        }

        public HttpResponseMessage Logout()
        {
            var response = Request.CreateResponse(HttpStatusCode.NoContent);
            response.Headers.AddCookies(new[] { CreateDeleteCookie() });
            return response;
        }

        public HttpResponseMessage Register(Registration registration)
        {
            if (registration == null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The registration request is missing a body.");
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

            var user = userService.GetUserByEmailAddress(registration.EmailAddress);
            if (user != null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The requested e-mail address is already registered.");

            user = DbUser.Create(registration.EmailAddress, registration.DisplayName, registration.Password);
            userService.AddUser(user);

            return Request.CreateResponse(HttpStatusCode.OK, new User(user));
        }

        // Helpers

        private HttpResponseMessage BadCredentials()
        {
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The provided credentials were incorrect.");
        }

        private CookieHeaderValue CreateAuthCookie(DbUser user, bool persistent)
        {
            var cookie = new CookieHeaderValue(AuthHelpers.AuthCookieName, AuthHelpers.Encode(user)) { HttpOnly = true };
            if (persistent)
                cookie.Expires = DateTimeOffset.UtcNow.AddYears(1);
            return cookie;
        }

        private CookieHeaderValue CreateDeleteCookie()
        {
            return new CookieHeaderValue(AuthHelpers.AuthCookieName, "") { Expires = DateTimeOffset.UtcNow.AddDays(-1), HttpOnly = true };
        }
    }
}