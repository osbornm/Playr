using System.Data.Entity;

namespace Playr.Web.Models
{
    public class PlayrContext : DbContext
    {
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Playr.Web.Models.PlayrContext>());

        public PlayrContext() : base("name=PlayrContext")
        {
        }

        public DbSet<UserToken> UserTokens { get; set; }
    }
}
