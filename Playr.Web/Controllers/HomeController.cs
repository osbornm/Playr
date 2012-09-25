using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using Playr.Web.Models;

namespace Playr.Web.Controllers
{
    //TODO: clean up controllers! Access controls, validation, the wroks!

    public class HomeController : Controller
    {

        public async Task<ActionResult> Index()
        {
            var client = new HttpClient();
            var request = CreateRequest(HttpMethod.Get, Helpers.BuildApiUrl("/Queue"), Request.IsAuthenticated);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return View(await response.Content.ReadAsAsync<JToken>());
        }

        [Authorize]
        public async Task<JToken> PlayPause()
        {
            using (var db = new PlayrContext())
            {
                var email = Membership.GetUser().Email;
                var userToken = db.UserTokens.FirstOrDefault(u => u.Email == email);
                if (userToken != null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Put,Helpers.BuildApiUrl("/PlayPause"));
                    request.Headers.Add("x-playr-token", userToken.Token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<JToken>();
                }
            }
            return null;
        }

        [Authorize]
        public async Task<JToken> Next()
        {
            using (var db = new PlayrContext())
            {
                var email = Membership.GetUser().Email;
                var userToken = db.UserTokens.FirstOrDefault(u => u.Email == email);
                if (userToken != null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, Helpers.BuildApiUrl("/next"));
                    request.Headers.Add("x-playr-token", userToken.Token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<JToken>();

                }
            }
            return null;
        }

        [Authorize]
        public async Task<JToken> Previous()
        {
            using (var db = new PlayrContext())
            {
                var email = Membership.GetUser().Email;
                var userToken = db.UserTokens.FirstOrDefault(u => u.Email == email);
                if (userToken != null)
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, Helpers.BuildApiUrl("/previous"));
                    request.Headers.Add("x-playr-token", userToken.Token);
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<JToken>();
                }
            }
            return null;
        }

        public async Task<JToken> GetQueue()
        {
            var client = new HttpClient();
            var request = CreateRequest(HttpMethod.Get,Helpers.BuildApiUrl("/Queue"), Request.IsAuthenticated);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<JToken>();
        }

        [Authorize]
        public async Task<JToken> favorite(int id)
        {
            var client = new HttpClient();
            var request = CreateRequest(HttpMethodTypes[Request.HttpMethod], Helpers.BuildApiUrl("/songs/" + id + "/favorite"), Request.IsAuthenticated);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<JToken>();
        }

        [HttpPost]
        public async Task<ActionResult> UploadFiles(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                await UploadFile(file);
            }
            return Json("All Files Uploaded");
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Upload(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                try
                {
                    await UploadFile(file);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            return RedirectToAction("index");
        }

        [Authorize]
        public ActionResult Admin()
        {
            return View();
        }

        private Dictionary<string, HttpMethod> HttpMethodTypes = new Dictionary<string, HttpMethod> { { "GET", HttpMethod.Get }, { "POST", HttpMethod.Post }, { "PUT", HttpMethod.Put }, { "DELETE", HttpMethod.Delete } };

        [NonAction]
        public static HttpRequestMessage CreateRequest(HttpMethod method, string Url, bool includeUserToken)
        {
            var request = new HttpRequestMessage(method, Url);
            if (includeUserToken)
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
            return request;
        }

        [NonAction]
        private async Task<JToken> UploadFile(HttpPostedFileBase file)
        {
            var client = new HttpClient();
            var request = CreateRequest(HttpMethod.Post, Helpers.BuildApiUrl("/upload"), true);
            var content = new StreamContent(file.InputStream);
            content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<JToken>();
        }
    }
}
