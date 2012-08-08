using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Playr.Web.Models;

namespace Playr.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5555/Queue");
                
            // If logged in send the Token
            if (Request.IsAuthenticated)
            {
                using (var db = new PlayrContext())
                {
                    var email = Membership.GetUser().Email;
                    var userToken = db.UserTokens.FirstOrDefault(u => u.Email == email);
                    if (userToken != null)
                    {
                        request.Headers.Add("x-playr-token", userToken.Token);
                    }
                }
            }

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return View(await response.Content.ReadAsAsync<JToken>());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<JToken> PlayPause()
        {
            using (var db = new PlayrContext())
            {
                var email = Membership.GetUser().Email;
                var userToken = db.UserTokens.FirstOrDefault(u => u.Email == email);
                if (userToken != null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Put, "http://localhost:5555/PlayPause");
                    request.Headers.Add("x-playr-token", userToken.Token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<JToken>();
                }
            }
            return null;
        }

        public async Task<JToken> Next()
        {
            using (var db = new PlayrContext())
            {
                var email = Membership.GetUser().Email;
                var userToken = db.UserTokens.FirstOrDefault(u => u.Email == email);
                if (userToken != null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5555/next");
                    request.Headers.Add("x-playr-token", userToken.Token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<JToken>();

                }
            }
            return null;
        }

        public async Task<JToken> Previous()
        {
            using (var db = new PlayrContext())
            {
                var email = Membership.GetUser().Email;
                var userToken = db.UserTokens.FirstOrDefault(u => u.Email == email);
                if (userToken != null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5555/previous");
                    request.Headers.Add("x-playr-token", userToken.Token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<JToken>();
                }
            }
            return null;
        }
    }
}
