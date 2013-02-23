using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using Playr.Api.Library.Models;
using Playr.DataModels;

namespace Playr.Api.Music.Models
{
    public class CurrentTrack
    {
        public CurrentTrack(DbAlbum album, DbTrack track, UrlHelper url)
        {
            Track = new Track(track, url);

            // TODO: Figure out default background strategy
            Fanart = Enumerable.Empty<string>();

            if (Program.FanartEnabled)
            {
                try
                {
                    var fanartFolder = Path.Combine(Program.FanArtworkPath, album.ArtistName);
                    Fanart = Directory.GetFiles(fanartFolder)
                                      .Select(path => url.LinkToArtistFanart(album.ArtistName, Path.GetFileNameWithoutExtension(path)));
                }
                catch { }
            }
        }

        public IEnumerable<string> Fanart { get; set; }

        public Track Track { get; set; }
    }
}
