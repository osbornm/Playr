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

            Index(album => album.ArtistName, FieldIndexing.Analyzed);
            Index(album => album.Name, FieldIndexing.Analyzed);
        }
    }
}
