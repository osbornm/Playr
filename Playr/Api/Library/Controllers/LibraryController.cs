using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Ionic.Zip;
using Playr.Api.Library.Models;

namespace Playr.Api.Library.Controllers
{
    public class LibraryController : MusicLibraryControllerBase
    {
        public MusicLibrary GetLibraryInfo()
        {
            return new MusicLibrary(MusicLibraryService.GetLibraryInfo(), Url);
        }

        [Authorize]
        public async Task<HttpResponseMessage> PostTracks()
        {
            var result = new UploadResult();
            var content = Request.Content;

            if (content.Headers.ContentType.IsMultipartFormData())
            {
                var provider = new MultipartFormDataStreamProvider(Program.TempPath);
                await content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                    ProcessFile(file.LocalFileName, file.Headers.ContentType, result);
            }
            else
            {
                var fileName = Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N"));
                await content.ReadAsFileAsync(fileName);
                ProcessFile(fileName, Request.Content.Headers.ContentType, result);
            }

            return Request.CreateResponse(
                result.Tracks.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.UnsupportedMediaType,
                result
            );
        }

        /// <summary>
        /// This version of ProcessFile assumes the filename has the correct extension.
        /// </summary>
        private void ProcessFile(string fileName, UploadResult result)
        {
            Console.WriteLine("> Processing: {0}", fileName);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".zip":
                    ProcessFile_Zip(fileName, result);
                    break;

                case ".mp3":
                case ".m4a":
                    ProcessFile_Audio(fileName, result);
                    break;

                default:
                    result.Errors.Add(String.Format("Unsupported media type for file {0} (only .mp3, .m4a, and .zip are supported)", Path.GetFileName(fileName)));
                    break;
            }
        }

        /// <summary>
        /// This version of ProcessFile ensures that the file extension matches the media type.
        /// </summary>
        private void ProcessFile(string fileName, MediaTypeHeaderValue mediaType, UploadResult result)
        {
            var extension = mediaType.ToFileExtension();
            var newPath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + extension);

            if (newPath != fileName)
            {
                File.Move(fileName, newPath);
                fileName = newPath;
            }

            ProcessFile(fileName, result);
        }

        private void ProcessFile_Audio(string fileName, UploadResult result)
        {
            try
            {
                var dbTrack = MusicLibraryService.AddFile(fileName);
                result.Tracks.Add(new Track(dbTrack, Url));
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }
        }

        private void ProcessFile_Zip(string zipFile, UploadResult result)
        {
            try
            {
                var outputPath = Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N"));
                PathHelpers.EnsurePathExists(outputPath, forceClean: true);

                using (var zip = ZipFile.Read(zipFile))
                    zip.ExtractSelectedEntries("name = *.m4a or name = *.mp3 or name = *.zip", null, outputPath);

                ProcessFolder(outputPath, result);

                SwallowExceptions(() => File.Delete(zipFile));
                SwallowExceptions(() => Directory.Delete(outputPath, recursive: true));
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }
        }

        private void ProcessFolder(string folder, UploadResult result)
        {
            Console.WriteLine("> Searching: {0}", folder);

            foreach (var file in Directory.GetFiles(folder))
                ProcessFile(file, result);

            foreach (var subFolder in Directory.GetDirectories(folder))
                ProcessFolder(subFolder, result);
        }

        private void SwallowExceptions(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }

        class UploadResult
        {
            public UploadResult()
            {
                Errors = new List<string>();
                Tracks = new List<Track>();
            }

            public List<string> Errors { get; private set; }

            public List<Track> Tracks { get; private set; }
        }
    }
}
