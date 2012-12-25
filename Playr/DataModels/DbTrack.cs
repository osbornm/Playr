using System;

namespace Playr.DataModels
{
    public class DbTrack : DbModel
    {
        public int AlbumId { get; set; }
        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public int AudioBitrate { get; set; }
        public int AudioChannels { get; set; }
        public int AudioSampleRate { get; set; }
        public uint BeatsPerMinute { get; set; }
        public string Composer { get; set; }
        public string Conductor { get; set; }
        public uint DiscNumber { get; set; }
        public int FavoriteCount { get; set; }
        public DateTimeOffset LastPlayed { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public int Plays { get; set; }
        public int SkipCount { get; set; }
        public TimeSpan Time { get; set; }
        public uint TrackNumber { get; set; }
        public uint Year { get; set; }
    }
}
