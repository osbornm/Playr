using System;
using System.Security.Cryptography;
using System.Text;

namespace Playr.DataModels
{
    public class DbUser : DbModel
    {
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string ApiToken { get; set; }
        public string HashedPassword { get; private set; }
        public string PasswordSalt { get; private set; }

        public static DbUser Create(string emailAddress, string displayName, string password)
        {
            var pretzel = new byte[32];

            using (var csp = RNGCryptoServiceProvider.Create())
                csp.GetBytes(pretzel);

            return new DbUser
            {
                EmailAddress = emailAddress,
                DisplayName = displayName,
                PasswordSalt = Convert.ToBase64String(pretzel),
            }
            .SetPassword(password)
            .GenerateApiToken();
        }

        public DbUser GenerateApiToken()
        {
            ApiToken = Guid.NewGuid().ToString();
            return this;
        }

        private string GetHashedPassword(string pwd)
        {
            if (String.IsNullOrWhiteSpace(pwd))
                return null;

            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(PasswordSalt + pwd + "8sdTEWK4$nc582n49#"));
                return Convert.ToBase64String(computedHash);
            }
        }

        public DbUser SetPassword(string pwd)
        {
            HashedPassword = GetHashedPassword(pwd);
            return this;
        }

        public bool ValidatePassword(string password)
        {
            return HashedPassword == GetHashedPassword(password);
        }
    }
}