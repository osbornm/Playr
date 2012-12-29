using System.Linq;
using Raven.Client.Indexes;

namespace Playr.DataModels.Indexes
{
    public class DbAlbum_Artists : AbstractIndexCreationTask<DbAlbum, DbAlbum_Artists.Result>
    {
        public DbAlbum_Artists()
        {
            Map = albums => albums.Select(album => new Result { ArtistName = album.ArtistName });

            // Using .ToLowerInvariant because Raven's GroupBy projection does not appear to support
            // the notion of passing a comparer to GroupBy (since it implicitly converts everything
            // to dynamic). As such, the Select needs to re-find the first Genre, so we can preserve
            // the presumed common capitalization.
            Reduce = results => results.GroupBy(result => result.ArtistName.ToLowerInvariant())
                                       .Select(groups => new Result { ArtistName = groups.First().ArtistName });
        }

        public class Result
        {
            public string ArtistName { get; set; }
        }
    }
}