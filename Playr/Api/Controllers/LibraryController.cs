using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Playr.Api.Models;
using Playr.DataModels;

namespace Playr.Api.Controllers
{
    public class LibraryController : MusicLibraryControllerBase
    {
        public Library GetLibraryInfo()
        {
            return new Library(MusicLibraryService.GetLibraryInfo(), Url);
        }

        public async Task<HttpResponseMessage> PostTracks()
        {
            try
            {
                var mediaType = Request.Content.Headers.ContentType;

                List<DbTrack> tracks = new List<DbTrack>();

                if (mediaType.IsMultipartFormData())
                {
                    var provider = new MultipartFormDataStreamProvider(Program.TempPath);
                    await Request.Content.ReadAsMultipartAsync(provider);

                    foreach (var file in provider.FileData.Where(f => f.Headers.ContentType.IsAudio()))
                    {
                        tracks.Add(MusicLibraryService.AddFile(file.LocalFileName, file.Headers.ContentType));
                    }
                }

#if false
            // Did they upload a zip file? 
            if (mediaType.Equals("application/x-zip-compressed", StringComparison.InvariantCultureIgnoreCase))
            {
                // We assume this is going to be a large file lets copy it to disc
                var tempFile = Path.Combine(ApplicationSettings.TempPath, string.Format("{0}.zip", Guid.NewGuid()));
                using (var fileStream = File.Create(tempFile))
                {
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                    stream.Close();
                }

                // Extract all the audio files for the temp file and copy them to iTunes Add Folder.
                using (var zip = ZipFile.Read(tempFile))
                {
                    zip.ExtractSelectedEntries("name = *.m4a or name = *.mp3 or name = *.aac or name = *.wav", String.Empty, ApplicationSettings.iTunesAddFolder);
                }

                // Delete the temp zip file we created
                File.Delete(tempFile);
            }
#endif

                // Did they upload just a single file?
                else if (mediaType.IsAudio())
                {
                    var tempFile = Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N"));
                    await Request.Content.ReadAsFileAsync(tempFile, true);
                    tracks.Add(MusicLibraryService.AddFile(tempFile, mediaType));
                }

                if (tracks.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.Accepted, tracks);
                }

                // Well they uploaded something we don't support!
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "The file type is unsupported.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
