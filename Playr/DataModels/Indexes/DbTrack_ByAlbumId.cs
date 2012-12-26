using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Playr.DataModels.Indexes
{
    public class DbTrack_ByAlbumId : AbstractIndexCreationTask<DbTrack>
    {
        public DbTrack_ByAlbumId()
        {
            Map = tracks => tracks.Select(track => new { track.AlbumId });

            Index(track => track.AlbumId, FieldIndexing.Analyzed);
        }
    }
}
