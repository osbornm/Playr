using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Ionic.Zip;
using Playr.Api.Library.Models;
using Playr.DataModels;

namespace Playr.Api.Library.Controllers
{
    public class LibraryController : MusicLibraryControllerBase
    {
        public MusicLibrary GetLibraryInfo()
        {
            return new MusicLibrary(MusicLibraryService.GetLibraryInfo(), Url);
        }

        //[Authorize]
        public async Task<HttpResponseMessage> PostTracks()
        {
            var content = Request.Content;

            if (content.Headers.ContentType.IsMultipartFormData())
            {
                var provider = new MultipartFormDataStreamProvider(Program.TempPath);
                await content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                    ThreadPool.QueueUserWorkItem(_ => ProcessFile(file.LocalFileName, file.Headers.ContentType));
            }
            else
            {
                var fileName = Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N"));
                await content.ReadAsFileAsync(fileName);
                ThreadPool.QueueUserWorkItem(_ => ProcessFile(fileName, Request.Content.Headers.ContentType));
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, "Thanks, brah");
        }

        /// <summary>
        /// This version of ProcessFile assumes the filename has the correct extension.
        /// </summary>
        private void ProcessFile(string fileName)
        {
            Console.WriteLine("> Processing: {0}", fileName);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".zip":
                    ProcessFile_Zip(fileName);
                    break;

                case ".mp3":
                case ".m4a":
                    ProcessFile_Audio(fileName);
                    break;

                default:
                    Console.WriteLine("Unsupported file type: {0}", Path.GetFileName(fileName));
                    File.Delete(fileName);
                    break;
            }
        }

        /// <summary>
        /// This version of ProcessFile ensures that the file extension matches the media type.
        /// </summary>
        private void ProcessFile(string fileName, MediaTypeHeaderValue mediaType)
        {
            var extension = mediaType.ToFileExtension();
            var newPath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + extension);

            if (newPath != fileName)
            {
                File.Move(fileName, newPath);
                fileName = newPath;
            }

            ProcessFile(fileName);
        }

        private void ProcessFile_Audio(string fileName)
        {
            try
            {
                MusicLibraryService.AddFile(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }

        private void ProcessFile_Zip(string zipFile)
        {
            try
            {
                var outputPath = Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N"));
                PathHelpers.EnsurePathExists(outputPath, forceClean: true);

                using (var zip = ZipFile.Read(zipFile))
                    zip.ExtractSelectedEntries("name = *.m4a or name = *.mp3 or name = *.zip", null, outputPath);

                ProcessFolder(outputPath);

                SwallowExceptions(() => File.Delete(zipFile));
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex.Message);
            }
        }

        private void ProcessFolder(string folder)
        {
            Console.WriteLine("> Searching: {0}", folder);

            foreach (var file in Directory.GetFiles(folder))
                ThreadPool.QueueUserWorkItem(_ => ProcessFile(file));

            foreach (var subFolder in Directory.GetDirectories(folder))
                ProcessFolder(subFolder);
        }

        private void SwallowExceptions(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }
    }
}
