using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Playr.Api.Models;
using Raven.Client.Embedded;

namespace Playr.Api.Controller
{
    public class UsersController : ApiController
    {
        public User Register(User u)
        {
            if (u == null || String.IsNullOrEmpty(u.Email) || String.IsNullOrEmpty(u.Name))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Must Pass in Email and Name."));
            }

            using (var session = Helpers.DocumentStore.OpenSession())
            {
                try
                {
                    session.Advanced.UseOptimisticConcurrency = true;
                    session.Store(new User { Email = u.Email, Name = u.Name, Token = Guid.NewGuid().ToString() }, "Users/" + u.Email);
                    session.SaveChanges();
                    return session.Load<User>("Users/" + u.Email);
                }
                catch
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Conflict, "There is already a user with that email."));
                }
            }
        }

        [HttpGet]
        public User Find(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Must provide an email."));
            }

            using (var session = Helpers.DocumentStore.OpenSession())
            {
                var user = session.Load<User>("Users/" + email);
                if (user == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such user"));
                }
                return user;
            }
        }

        public User ResetToken(User u)
        {
            if (u == null || String.IsNullOrEmpty(u.Email) || String.IsNullOrEmpty(u.Token))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Must Pass in Email and Toekn."));
            }

            using (var session = Helpers.DocumentStore.OpenSession())
            {
                var user = session.Load<User>("Users/" + u.Email);
                if (user == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such user"));
                }
                if (u.Token != user.Token)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "No can do"));
                }
                user.Token = Guid.NewGuid().ToString();
                session.SaveChanges();
                return user;
            }
        }
    }
}
