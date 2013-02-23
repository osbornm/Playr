using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Playr.Api.Library.Models;

namespace Playr.Api.Library.Controllers
{
    public class ArtistsController : MusicLibraryControllerBase
    {
        public List<Artist> GetArtists()
        {
            return MusicLibraryService.GetArtists()
                                      .Select(artist => new Artist(artist, Url))
                                      .ToList();
        }

        public Artist GetArtist(string artistName)
        {
            var albums = MusicLibraryService.GetAlbumsByArtist(artistName);
            if (albums.Count == 0)
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));

            return new Artist(artistName, Url);
        }

        public List<Album> GetAlbumsByArtist(string artistName)
        {
            return MusicLibraryService.GetAlbumsByArtist(artistName)
                                      .Select(album => new Album(album, Url))
                                      .ToList();
        }

        public HttpResponseMessage GetFanart(string artistName, string fanartId)
        {
            var artwork = Path.Combine(Program.FanArtworkPath, PathHelpers.ToFolderName(artistName), String.Format("{0}.jpg", fanartId));
            if (!File.Exists(artwork))
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var file = new FileStream(artwork, FileMode.Open);

            var response = Request.CreateResponse();
            response.Content = new StreamContent(file);
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.MaxAge = TimeSpan.FromHours(48);
            response.Headers.CacheControl.MustRevalidate = true;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return response;
        }
    }
}