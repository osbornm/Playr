using System.Linq;
using Raven.Client.Indexes;

namespace Playr.DataModels.Indexes
{
    public class DbAlbum_Genres : AbstractIndexCreationTask<DbAlbum, DbAlbum_Genres.Result>
    {
        public DbAlbum_Genres()
        {
            Map = albums => albums.Select(album => new Result { Genre = album.Genre });

            // Using .ToLowerInvariant because Raven's GroupBy projection does not appear to support
            // the notion of passing a comparer to GroupBy (since it implicitly converts everything
            // to dynamic). As such, the Select needs to re-find the first Genre, so we can preserve
            // the presumed common capitalization.
            Reduce = results => results.GroupBy(result => result.Genre.ToLowerInvariant())
                                       .Select(groups => new Result { Genre = groups.First().Genre });
        }

        public class Result
        {
            public string Genre { get; set; }
        }
    }
}