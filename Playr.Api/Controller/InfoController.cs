using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using iTunesLib;
using Playr.Api.Models;
using Raven.Client.Embedded;

namespace Playr.Api.Controller
{
    public class InfoController: ApiController
    {
        iTunesAppClass itunes;

        public InfoController()
        {
            itunes = new iTunesAppClass();
        }

        [HttpGet]
        public Song CurrentTrack()
        {
            return itunes.CurrentTrack.toSong(Url);
        }

        [HttpGet]
        public Queue Queue()
        {
            var tracks = itunes.CurrentPlaylist.Tracks;

            var bar = new Queue();

            for (int i = 1; i < 6; i++)
            {
                bar.PreviouslyPlayed.Add(tracks[i].toSong(Url));
            }

            bar.CurrentTrack = tracks[6].toSong(Url);

            for (int i = 7; i <= tracks.Count; i++)
            {
                bar.UpNext.Add(tracks[i].toSong(Url));
            }

            return bar;
        }


        [HttpGet]
        public HttpResponseMessage Artwork(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            var track = itunes.GetTrackById(id);

            if (track.Artwork.Count > 0)
            {
                var fileName = track.TrackDatabaseID + ".jpeg";
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "artwork\\");
                var fullPath = Path.Combine(path, fileName);
                if (!File.Exists(fullPath))
                {
                    // TODO: Put this is start up? 
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    track.Artwork[1].SaveArtworkToFile(fullPath.ToString());
                }
                var file = new FileStream(fullPath.ToString(), FileMode.Open);
                response.Content = new StreamContent(file);
            }
            else
            {
                var stream = new MemoryStream();
                Playr.Api.Resources.NoArt.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;
                response.Content = new StreamContent(stream);
            }

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.MaxAge = TimeSpan.FromHours(24);
            response.Headers.CacheControl.MustRevalidate = true;
            return response;
        }


        [RequireToken]
        public void FavoriteTrack(Song s)
        {
            using (var session = Helpers.DocumentStore.OpenSession())
            {
                var token = Request.GetToken();
                var user = session.Query<User>().Where(u => u.Token == token).First();

                var song = itunes.GetTrackById(s.Id);
                if (song == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such song"));
                }
                song.Rating += 5;
                user.Favorites.Add(s);
                session.SaveChanges();
            }
        }
    }
}
