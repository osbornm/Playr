using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using Playr.Api.Library.Models;
using Playr.DataModels;
using Playr.Models;

namespace Playr.Api.Music.Models
{
    public class CurrentTrack
    {
        public CurrentTrack(DbAlbum album, DbTrack track, double currentTime, TrackState audioState)
        {
            Track = new Track(track);
            CurrentTime = currentTime;
            State = audioState;

            Fanart = Enumerable.Empty<string>();
            try
            {
                var fanartFolder = Path.Combine(Program.FanArtworkPath, album.ArtistName);
                Fanart = Directory.GetFiles(fanartFolder)
                                    .Select(path => RouteLinks.LinkToArtistFanart(album.ArtistName, Path.GetFileNameWithoutExtension(path)));
            }
            catch { }
        }

        public IEnumerable<string> Fanart { get; set; }
        public double CurrentTime { get; set; }
        public Track Track { get; set; }
        public TrackState State { get; set; }
    }
}
