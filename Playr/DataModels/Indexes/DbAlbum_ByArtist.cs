using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Playr.DataModels.Indexes
{
    public class DbAlbum_ByArtist : AbstractIndexCreationTask<DbAlbum>
    {
        public DbAlbum_ByArtist()
        {
            Map = albums => albums.Select(album => new { ArtistName = album.ArtistName.ToLowerInvariant() });

            Index(album => album.ArtistName, FieldIndexing.Analyzed);
        }
    }
}
