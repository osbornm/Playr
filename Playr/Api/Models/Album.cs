using Playr.DataModels;

namespace Playr.Api
{
    public class Album : SelfLinkModel
    {
        public Album() { }

        public Album(DbAlbum album, string selfLink)
        {
            _Self = selfLink;
            ArtistName = album.ArtistName;
            Genre = album.Genre;
            Name = album.Name;
        }

        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
    }
}
