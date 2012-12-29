using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Ionic.Zip;

namespace Playr.Api.Controllers
{
    public class DownloadController : MusicLibraryControllerBase
    {
        [HttpGet]
        public HttpResponseMessage Album(int id)
        {
            var album = MusicLibraryService.GetAlbumById(id);
            if (album == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            // HACK: Putting together the path here by hand feels wrong. Should the database store the path?
            string path = Path.Combine(Program.MusicLibraryPath, PathHelpers.ToFolderName(album.ArtistName), PathHelpers.ToFolderName(album.Name));
            if (!Directory.Exists(path))
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Could not find path: " + path);

            return SendFile(Zip(path), String.Format("{0} - {1}.zip", album.ArtistName, album.Name));
        }

        [HttpGet]
        public HttpResponseMessage Artist(string artistName)
        {
            // HACK: Putting together the path here by hand feels wrong. Should the database store the path?
            string path = Path.Combine(Program.MusicLibraryPath, PathHelpers.ToFolderName(artistName));
            if (!Directory.Exists(path))
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return SendFile(Zip(path), artistName + ".zip");
        }

        [HttpGet]
        public HttpResponseMessage Track(int id)
        {
            var track = MusicLibraryService.GetTrackById(id);
            if (track == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            return SendFile(track.Location, String.Format("{0} - {1}{2}", track.ArtistName, track.Name, Path.GetExtension(track.Location)));
        }

        private HttpResponseMessage SendFile(string filename, string attachmentName)
        {
            var stream = File.Open(filename, FileMode.Open, FileAccess.Read);
            var content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = attachmentName };

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = content;
            return response;
        }

        private string Zip(string path)
        {
            TemporaryZipFile result = new TemporaryZipFile();
            Request.RegisterForDispose(result);

            using (ZipFile zipFile = new ZipFile(result.Path))
            {
                zipFile.AddDirectory(path);
                zipFile.Save();
            }

            return result.Path;
        }

        private class TemporaryZipFile : IDisposable
        {
            public TemporaryZipFile()
            {
                Path = System.IO.Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N") + ".zip");
            }

            public string Path { get; private set; }

            public void Dispose()
            {
                try
                {
                    if (File.Exists(Path))
                        File.Delete(Path);
                }
                catch { }
            }
        }
    }
}
