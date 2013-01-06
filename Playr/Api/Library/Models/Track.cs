using System;
using System.Web.Http.Routing;
using Playr.Api.Shared.Models;
using Playr.DataModels;

namespace Playr.Api.Library.Models
{
    public class Track : ModelWithLinks
    {
        public Track() { }

        public Track(DbTrack track, UrlHelper url)
        {
            AddLink("album", url.LinkToAlbum(track.AlbumId));
            AddLink("download", url.LinkToTrackDownload(track));

            AlbumName = track.AlbumName;
            AudioBitrate = track.AudioBitrate;
            AudioChannels = track.AudioChannels;
            AudioSampleRate = track.AudioSampleRate;
            BeatsPerMinute = track.BeatsPerMinute;
            Composer = track.Composer;
            Conductor = track.Conductor;
            DiscNumber = track.DiscNumber;
            LastPlayed = track.LastPlayed;
            Name = track.Name;
            Plays = track.Plays;
            SkipCount = track.SkipCount;
            Time = track.Time;
            TrackNumber = track.TrackNumber;
            Year = track.Year;
        }

        // TODO: Expose Rating as a function of favorite count vs. user count

        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public int AudioBitrate { get; set; }
        public int AudioChannels { get; set; }
        public int AudioSampleRate { get; set; }
        public uint BeatsPerMinute { get; set; }
        public string Composer { get; set; }
        public string Conductor { get; set; }
        public uint DiscNumber { get; set; }
        public DateTimeOffset LastPlayed { get; set; }
        public string Name { get; set; }
        public int Plays { get; set; }
        public int SkipCount { get; set; }
        public TimeSpan Time { get; set; }
        public uint TrackNumber { get; set; }
        public uint Year { get; set; }
    }
}
