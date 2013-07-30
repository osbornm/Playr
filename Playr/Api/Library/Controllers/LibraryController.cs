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
                    ThreadPool.QueueUserWorkItem(_ => MusicLibraryService.ProcessFile(file.LocalFileName, file.Headers.ContentType));
            }
            else
            {
                var fileName = Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N"));
                await content.ReadAsFileAsync(fileName);
                ThreadPool.QueueUserWorkItem(_ => MusicLibraryService.ProcessFile(fileName, Request.Content.Headers.ContentType));
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, "Thanks, brah");
        }

    }
}
