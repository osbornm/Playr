using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Playr.DataModels.Indexes
{
    public class DbAlbum_ByGenre : AbstractIndexCreationTask<DbAlbum>
    {
        public DbAlbum_ByGenre()
        {
            Map = albums => albums.Select(album => new { Genre = album.Genre.ToLowerInvariant() });

            Index(album => album.Genre, FieldIndexing.Analyzed);
        }
    }
}
