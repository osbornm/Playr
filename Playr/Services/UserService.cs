using Playr.DataModels;

namespace Playr.Services
{
    public class UserService
    {
        public void AddUser(DbUser user)
        {
            using (var session = Database.OpenSession())
            {
                string userId = "DbUsers/" + user.EmailAddress.ToLowerInvariant();

                session.Advanced.UseOptimisticConcurrency = true;
                session.Store(user, userId);
                session.Store(new Reference { RefId = userId }, "DbUsers/ApiTokens/" + user.ApiToken.ToLowerInvariant());
                session.SaveChanges();
            }
        }

        public DbUser GetUserByApiToken(string apiToken)
        {
            using (var session = Database.OpenSession())
            {
                var userReference = session.Load<Reference>("DbUsers/ApiTokens/" + apiToken.ToLowerInvariant());
                if (userReference == null)
                    return null;

                return session.Load<DbUser>(userReference.RefId);
            }
        }

        public DbUser GetUserByEmailAddress(string emailAddress)
        {
            using (var session = Database.OpenSession())
                return session.Load<DbUser>("DbUsers/" + emailAddress.ToLowerInvariant());
        }

        class Reference
        {
            public string RefId { get; set; }
        }
    }
}