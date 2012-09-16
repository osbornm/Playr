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
using Ionic.Zip;
using iTunesLib;
using Playr.Api.Models;
using Raven.Client.Embedded;

namespace Playr.Api.Controller
{
    public class InfoController : ApiController
    {
        iTunesAppClass itunes;

        public InfoController()
        {
            itunes = new iTunesAppClass();
        }

        [HttpGet]
        public Song CurrentTrack()
        {
            var song = itunes.CurrentTrack.toSong();
            var token = Request.GetToken();
            if (!String.IsNullOrEmpty(token))
            {
                User user = null;
                using (var session = Helpers.DocumentStore.OpenSession())
                {
                    user = session.Query<User>().Where(u => u.Token == token).FirstOrDefault();
                }
                if (user != null)
                {
                    song.IsFavorite = user.Favorites.Where(s => s.Id == song.Id).Any();
                }
            }

            return song;
        }

        [HttpGet]
        public DjInfo Queue()
        {
            return GetQueue();
        }

        [RequireToken, HttpPut]
        public DjInfo QueueSong(int id)
        {
            var track = itunes.GetTrackById(id);
            dynamic playlist = itunes.CurrentPlaylist;
            playlist.AddTrack(track);
            return GetQueue();
        }

        [HttpGet]
        public HttpResponseMessage Artwork(int id)
        {
            HttpResponseMessage response = Request.CreateResponse();
            var track = itunes.GetTrackById(id);

            if (track == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no song with that ID."));
            }

            if (track.Artwork.Count > 0)
            {
                var fileName = track.TrackDatabaseID + ".jpeg";
                var path = Path.Combine(ApplicationSettings.ArtworkFolder, fileName);
                if (!File.Exists(path))
                {
                    track.Artwork[1].SaveArtworkToFile(path.ToString());
                }
                var file = new FileStream(path, FileMode.Open);
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

        [HttpGet]
        public HttpResponseMessage DownloadSong(int id)
        {
            HttpResponseMessage response = Request.CreateResponse();
            var track = itunes.GetTrackById(id);

            if (track == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no song with that ID."));
            }

            var file = new FileStream(((IITFileOrCDTrack)track).Location, FileMode.Open);
            response.Content = new StreamContent(file);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = Path.GetFileName(((IITFileOrCDTrack)track).Location) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            response.Headers.CacheControl = new CacheControlHeaderValue();
            response.Headers.CacheControl.MaxAge = TimeSpan.FromHours(24);
            response.Headers.CacheControl.MustRevalidate = true;
            return response;
        }

        [HttpGet]
        public HttpResponseMessage DownloadAlbum(string name)
        {
            HttpResponseMessage response = Request.CreateResponse();
            var tracks = itunes.GetAlbumTracks(name);

            if (!tracks.Any())
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no album by that name."));
            }

            using (ZipFile zip = new ZipFile())
            {
                foreach (var t in tracks)
                {
                    zip.AddFile(t.Location, String.Empty);
                }
                zip.Name = name;
                var stream = new MemoryStream();
                zip.Save(stream);
                stream.Position = 0;
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                response.Headers.CacheControl = new CacheControlHeaderValue();
                response.Headers.CacheControl.MaxAge = TimeSpan.FromHours(24);
                response.Headers.CacheControl.MustRevalidate = true;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = String.Format("{0}.zip", name) };
                return response;
            }
        }

        [RequireToken, HttpPut, ActionName("favorite")]
        public void FavoriteSong(int id)
        {
            using (var session = Helpers.DocumentStore.OpenSession())
            {
                var token = Request.GetToken();
                var user = session.Query<User>().Where(u => u.Token == token).First();

                var track = itunes.GetTrackById(id);
                if (track == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such song"));
                }

                if (!user.Favorites.Where(fav => fav.Id==id).Any())
                {
                    track.Rating += 5;
                    user.Favorites.Add(track.toSong());
                    session.SaveChanges();
                }
            }
        }

        [RequireToken, HttpDelete, ActionName("favorite")]
        public void UnfavoriteSong(int id)
        {
            using (var session = Helpers.DocumentStore.OpenSession())
            {
                var token = Request.GetToken();
                var user = session.Query<User>().Where(u => u.Token == token).First();

                var track = itunes.GetTrackById(id);
                if (track == null)
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotFound, "No such song"));
                }

                if (user.Favorites.Where(fav => fav.Id == id).Any())
                {
                    track.Rating -= 5;
                    user.Favorites.RemoveAll(s => s.Id == id);
                    session.SaveChanges();
                }
            }
        }

        [RequireToken, HttpPost]
        public void Upload()
        {
            var mediaType = Request.Content.Headers.ContentType.MediaType;
            var task = Request.Content.ReadAsStreamAsync();
            task.Wait();

            // Did they upload a zip file? 
            if (mediaType.Equals("application/x-zip-compressed", StringComparison.InvariantCultureIgnoreCase))
            {
                // We assume this is going to be a large file lets copy it to disc
                var tempFile = Path.Combine(ApplicationSettings.TempPath, string.Format("{0}.zip", Guid.NewGuid()));
                using (var fileStream = File.Create(tempFile))
                {
                    task.Result.CopyTo(fileStream);
                    fileStream.Close();
                    task.Result.Close();
                }

                // Extract all the audio files for the temp file and copy them to iTunes Add Folder.
                using (var zip = ZipFile.Read(tempFile))
                {
                    zip.ExtractSelectedEntries("name = *.m4a or name = *.mp3 or name = *.aac or name = *.wav", String.Empty, ApplicationSettings.iTunesAddFolder);
                }

                // Delete the temp zip file we created
                File.Delete(tempFile);
            }

            // Did they upload just a single file?
            else if (mediaType.Equals("audio/mp4", StringComparison.InvariantCultureIgnoreCase) ||
                     mediaType.Equals("audio/m4a", StringComparison.InvariantCultureIgnoreCase) ||
                     mediaType.Equals("audio/mp3", StringComparison.InvariantCultureIgnoreCase) ||
                     mediaType.Equals("audio/wav", StringComparison.InvariantCultureIgnoreCase))
            {
                // TOOO: itunes is stupid and requires the extension for the file to be picked up. For now assume MIME type is audio/{extension} 
                var localFile = Path.Combine(ApplicationSettings.iTunesAddFolder, String.Format("{0}.{1}", Guid.NewGuid(), mediaType.Substring(6)));
                using (var fileStream = File.Create(localFile))
                {
                    task.Result.CopyTo(fileStream);
                    fileStream.Close();
                    task.Result.Close();
                }
            }

            // Well they uploaded something we don't support!
            else
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "The file type is unsupported."));
            }
        }
        
        [NonAction]
        public DjInfo GetQueue()
        {
            var token = Request.GetToken();
            User user = null;
            using (var session = Helpers.DocumentStore.OpenSession())
            {
                user = session.Query<User>().Where(u => u.Token == token).FirstOrDefault();
            }

            var tracks = itunes.CurrentPlaylist.Tracks;

            var queue = new DjInfo();

            var currentTrackReached = false;

            foreach (IITTrack track in tracks)
            {
                if (!currentTrackReached && track.TrackDatabaseID == itunes.CurrentTrack.TrackDatabaseID)
                {
                    currentTrackReached = true;
                    queue.CurrentTrack = track.toSong(user);
                }
                else if (currentTrackReached)
                {
                    queue.Queue.Add(track.toSong(user));
                } 
                else 
                {
                    queue.History.Add(track.toSong(user));
                }
            }
            return queue;
        }

    }
}
