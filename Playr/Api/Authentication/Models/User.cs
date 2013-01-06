using Playr.DataModels;

namespace Playr.Api.Authentication.Models
{
    public class User
    {
        public User(DbUser user)
        {
            ApiToken = user.ApiToken;
            DisplayName = user.DisplayName;
            EmailAddress = user.EmailAddress;
        }

        public string ApiToken { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
    }
}