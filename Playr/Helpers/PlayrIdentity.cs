using System.Security.Principal;
using Playr.DataModels;

public class PlayrIdentity : IIdentity
{
    public PlayrIdentity(string emailAddress, string name, string apiToken)
    {
        ApiToken = apiToken;
        EmailAddress = emailAddress;
        Name = name;
        Principal = new GenericPrincipal(this, new string[0]);
    }

    public PlayrIdentity(DbUser user)
    {
        ApiToken = user.ApiToken;
        EmailAddress = user.EmailAddress;
        Name = user.DisplayName;
        Principal = new GenericPrincipal(this, new string[0]);
    }

    public string ApiToken { get; private set; }

    public string AuthenticationType
    {
        get { return "Playr"; }
    }

    public string EmailAddress { get; private set; }

    public bool IsAuthenticated
    {
        get { return true; }
    }

    public string Name { get; private set; }

    public IPrincipal Principal { get; private set; }
}