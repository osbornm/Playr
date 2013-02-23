using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http.Routing;
using Playr.Api.Shared.Models;

namespace Playr.Api.Library.Models
{
    public class Artist : ModelWithLinks
    {
        public Artist(string name, UrlHelper url)
        {
            AddLink("albums", url.LinkToAlbumsByArtist(name));
            AddLink("download", url.LinkToArtistDownload(name));
            AddLink("self", url.LinkToArtist(name));

            Name = name;

            // TODO: Figure out default background strategy
            Fanart = Enumerable.Empty<string>();

            if (Program.FanartEnabled)
            {
                try
                {
                    var fanartFolder = Path.Combine(Program.FanArtworkPath, name);
                    Fanart = Directory.GetFiles(fanartFolder)
                                      .Select(path => url.LinkToArtistFanart(name, Path.GetFileNameWithoutExtension(path)));
                }
                catch { }
            }
        }

        public IEnumerable<string> Fanart { get; set; }
        public string Name { get; set; }
    }
}