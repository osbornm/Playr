namespace Playr.DataModels
{
    public class DbAlbum : DbModelWithId
    {
        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }

        public DbTrack[] Tracks { get; set; }
    }
}
