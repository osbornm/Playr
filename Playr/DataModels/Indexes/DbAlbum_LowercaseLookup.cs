using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Playr.DataModels.Indexes
{
    public class DbAlbum_LowercaseLookup : AbstractIndexCreationTask<DbAlbum>
    {
        public DbAlbum_LowercaseLookup()
        {
            Map = albums => albums.Select(album => new
                                          {
                                              ArtistName = album.ArtistName.ToLowerInvariant(),
                                              Name = album.Name.ToLowerInvariant(),
                                          });

            Index(x => x.ArtistName, FieldIndexing.Analyzed);
            Index(x => x.Name, FieldIndexing.Analyzed);
        }
    }
}
