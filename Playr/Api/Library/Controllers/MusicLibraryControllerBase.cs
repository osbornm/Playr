using System.Web.Http;
using Playr.Services;

namespace Playr.Api.Library.Controllers
{
    public abstract class MusicLibraryControllerBase : ApiController
    {
        public MusicLibraryControllerBase()
        {
            MusicLibraryService = new MusicLibraryService();
        }

        public MusicLibraryService MusicLibraryService { get; set; }
    }
}