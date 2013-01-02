using System;
using System.Threading;
using Playr.DataModels;

public static class AuthHelpers
{
    public const string AuthCookieName = "Playr-Authentication";
    public const string AuthHeaderName = "Playr-ApiToken";
    public const string AuthQueryStringName = "apitoken";

    public static PlayrIdentity CurrentUser
    {
        get
        {
            var principal = Thread.CurrentPrincipal;
            if (principal != null)
                return principal.Identity as PlayrIdentity;

            return null;
        }
        set
        {
            Thread.CurrentPrincipal = value.Principal;
        }
    }

    // TODO: Enryption/decryption

    public static PlayrIdentity Decode(string value)
    {
        string[] parts = value.Split('\n');
        if (parts.Length == 3)
            return new PlayrIdentity(parts[0], parts[1], parts[2]);

        return null;
    }

    public static string Encode(DbUser user)
    {
        return String.Format("{0}\n{1}\n{2}", user.EmailAddress, user.DisplayName, user.ApiToken);
    }
}