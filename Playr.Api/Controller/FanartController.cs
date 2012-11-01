using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Ionic.Zip;
using iTunesLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Playr.Api.Models;
using Raven.Client.Embedded;

namespace Playr.Api.Controller
{
    public class FanartController : ApiController
    {

        public static Dictionary<string, List<string>> fanart = new Dictionary<string, List<string>>();
        private static string _fanartUrlFormatString = ApplicationSettings.apiBaseUrl + "/artists/{0}/fanart/{1}";

        [HttpGet]
        public List<string> FanartCollection(string artist)
        {
            if (string.IsNullOrEmpty(artist))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Must give an artist name."));
            }

            if (!Helpers.IsFanArtEnabled())
            {
                return new List<string>();
            }

            if (!fanart.ContainsKey(artist))
            {
                var fanartTvUrls = GetLocalUrlsForArtist(artist);
                fanart[artist] = fanartTvUrls;

            }
            return fanart[artist];
        }

        [HttpGet]
        public HttpResponseMessage Fanart(string artist, string fileName)
        {
            HttpResponseMessage response = Request.CreateResponse();
            var path = Path.Combine(ApplicationSettings.FanArtFolder, artist, fileName);
            if (!File.Exists(path))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "Fanart Doesn't Exist."));
            }

            var file = new FileStream(path, FileMode.Open);
            response.Content = new StreamContent(file);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.MaxAge = TimeSpan.FromHours(24);
            response.Headers.CacheControl.MustRevalidate = true;
            return response;
        }

        [NonAction]
        private static string CreateDownloadUrl(string artist, string fileName)
        {
            return String.Format(_fanartUrlFormatString, HttpUtility.UrlEncode(artist), HttpUtility.UrlEncode(fileName));
        }

        [NonAction]
        private static List<string> GetUrlsForArtist(string artist)
        {
            var fanartUrls = new List<string>();

            // Search for the MID first
            var musicbarinzClient = new HttpClient();
            var musicbrainzUrl = String.Format("http://search.musicbrainz.org/ws/2/artist/?query={0}&fmt=json", HttpUtility.UrlEncode(artist));
            var musicbrainzResponseTask = musicbarinzClient.GetAsync(musicbrainzUrl);
            musicbrainzResponseTask.Wait();
            var musicbrainzResponse = musicbrainzResponseTask.Result;
            var musicbrainzTask = musicbrainzResponse.Content.ReadAsAsync<SearchResult>();
            musicbrainzTask.Wait();
            var searchResult = musicbrainzTask.Result;
            if (searchResult.artist_list != null && searchResult.artist_list.artist != null && searchResult.artist_list.artist.Count > 0)
            {
                var mid = searchResult.artist_list.artist.OrderByDescending(a => a.score).First().id;
                if (mid != null)
                {
                    // Once there is an ID get the FanArt
                    var client = new HttpClient();
                    var url = String.Format("http://fanart.tv/webservice/artist/c530881c38da5630652532a36dd8983a/{0}/JSON", HttpUtility.UrlEncode(mid));
                    var fanartTask = client.GetStringAsync(url);
                    fanartTask.Wait();
                    var raw = fanartTask.Result;
                    try
                    {
                        var obj = JObject.Parse(raw);
                        if (obj != null)
                        {
                            if (obj.HasValues)
                            {
                                var fanartArtist = (JProperty)obj.First;
                                if (fanartArtist.HasValues)
                                {
                                    var bgs = fanartArtist.Value["artistbackground"];
                                    foreach (var background in bgs)
                                    {
                                        fanartUrls.Add(background["url"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    catch { } // the fan art service sometimes has manformed JSON and can cause errors.
                }
            }
            return fanartUrls;
        }

        [NonAction]
        private static List<string> GetLocalUrlsForArtist(string artist)
        {
            var fanartFolder = Path.Combine(ApplicationSettings.FanArtFolder, artist);
            var localUrls = new List<string>();

            // Check to see if we have fanart already
            if (!Directory.Exists(fanartFolder))
            {
                Directory.CreateDirectory(fanartFolder);
            }

            foreach (var file in (new DirectoryInfo(fanartFolder)).GetFiles())
            {
                localUrls.Add(CreateDownloadUrl(artist, file.Name));
            }

            // If we don't have the files then try to get fanart from the services
            if (!localUrls.Any())
            {
                var tasks = new List<Task>();
                var fanartTvUrls = GetUrlsForArtist(artist);
                for (int i = 0; i < fanartTvUrls.Count; i++)
                {
                    var client = new HttpClient();
                    var task = client.GetAsync(fanartTvUrls[i]).ContinueWith(
                      (requestTask) =>
                      {
                          HttpResponseMessage response = requestTask.Result;
                          response.EnsureSuccessStatusCode();
                          var fileName = Guid.NewGuid() + ".jpg";
                          var filePath = Path.Combine(fanartFolder, fileName);
                          response.Content.ReadAsFileAsync(filePath, false).ContinueWith(
                          (readTask) =>
                          {
                              localUrls.Add(CreateDownloadUrl(artist, fileName));
                          });
                      });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            }
            return localUrls;
        }

        // Mapping classes to MusicBrainz JSON

        private class SearchResult
        {
            public string created { get; set; }
            [JsonProperty("artist-list")]
            public artistResults artist_list { get; set; }
        }

        private class artistResults
        {
            public List<artist> artist { get; set; }

        }

        private class artist
        {
            public int score { get; set; }
            public string id { get; set; }
        }
    }
}
