using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using Playr.Api.Library.Models;

namespace Playr.Api.Library.Controllers
{
    public class AlbumsController : MusicLibraryControllerBase
    {
        [HttpGet]
        public IEnumerable<Album> Albums()
        {
            return MusicLibraryService.GetAlbums()
                                      .Select(dbAlbum => new Album(dbAlbum, Url))
                                      .ToList();
        }

        [HttpGet]
        public Album Album(int id)
        {
            var dbAlbum = MusicLibraryService.GetAlbumById(id);
            if (dbAlbum == null)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return new Album(dbAlbum, Url);
        }

        [HttpGet]
        public HttpResponseMessage Artwork(int id)
        {
            HttpResponseMessage response = Request.CreateResponse();

            var albumArtwork = Path.Combine(Program.AlbumArtworkPath, String.Format("{0}.jpg", id));
            if (File.Exists(albumArtwork))
            {
                var file = new FileStream(albumArtwork, FileMode.Open);
                response.Content = new StreamContent(file);
                response.Headers.CacheControl = new CacheControlHeaderValue();
                response.Headers.CacheControl.MaxAge = TimeSpan.FromHours(48);
                response.Headers.CacheControl.MustRevalidate = true;
            }
            else
            {
                response.Content = new StreamContent(Assembly.GetExecutingAssembly().GetManifestResourceStream("Playr.Resources.CD.jpg"));
            }
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return response;
        }
    }
}