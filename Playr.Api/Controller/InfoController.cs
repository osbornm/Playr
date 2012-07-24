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

            var queue = new Queue();

            for (int i = 1; i < 6; i++)
            {
                queue.PreviouslyPlayed.Add(tracks[i].toSong(Url));
            }

            queue.CurrentTrack = tracks[6].toSong(Url);

            for (int i = 7; i <= tracks.Count; i++)
            {
                queue.UpNext.Add(tracks[i].toSong(Url));
            }

            return queue;
        }

        [HttpGet]
        public HttpResponseMessage Artwork(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
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
            HttpResponseMessage response = new HttpResponseMessage();
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
        public HttpResponseMessage DownloadAlbum(string album)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            var tracks = itunes.GetAlbumTracks(album);

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
                zip.Name = album;
                var stream = new MemoryStream();
                zip.Save(stream);
                stream.Position = 0;
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
                response.Headers.CacheControl = new CacheControlHeaderValue();
                response.Headers.CacheControl.MaxAge = TimeSpan.FromHours(24);
                response.Headers.CacheControl.MustRevalidate = true;
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = String.Format("{0}.zip", album) };
                return response;
            }
        }

        [RequireToken]
        public void FavoriteTrack(Song s)
        {
            // TODO: Don't allow a user favorite a song more than once
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

        [HttpPost]
        public void Upload()
        {
            HttpRequestMessage request = this.Request;
            var mediaType = request.Content.Headers.ContentType.MediaType;

            // Replace the network stream with a memory stream.
            // TODO: We should porbably read this to a file, I anticipate these uploads being pretty large...
            request.Content.LoadIntoBufferAsync();
            using (var task = request.Content.ReadAsStreamAsync())
            {
                task.Wait();

                // Did they upload a zip file? 
                if (mediaType.Equals("application/x-zip-compressed", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var zip = ZipFile.Read(task.Result))
                    {
                        // Extract all the audio files into the iTunes Add folder.
                        zip.ExtractSelectedEntries("name = *.m4a or name = *.mp3 or name = *.aac or name = *.wav", String.Empty, ApplicationSettings.iTunesAddFolder);
                    }
                }
                // Did they upload just a single file?
                else if (mediaType.Equals("audio/mp4", StringComparison.InvariantCultureIgnoreCase) ||
                         mediaType.Equals("audio/m4a", StringComparison.InvariantCultureIgnoreCase) ||
                         mediaType.Equals("audio/mp3", StringComparison.InvariantCultureIgnoreCase) ||
                         mediaType.Equals("audio/wav", StringComparison.InvariantCultureIgnoreCase))
                {
                    // TOOO: itunes it stupid and requires and extension for the file to be picked up. For now assume MIME type is audio/{extension} 
                    var localFile = Path.Combine(ApplicationSettings.iTunesAddFolder, String.Format("{0}.{1}", Guid.NewGuid(), mediaType.Substring(6)));
                    using (var fileStream = File.Create(localFile))
                    {
                        task.Result.CopyTo(fileStream);
                        fileStream.Close();
                    }
                }
                // Well they uploaded something we don't support!
                else
                {
                    throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "The file type is unsupported."));
                }

            }
        }
    }
}
