using System.Web.Http.Routing;

public static class UrlHelperExtensions
{
    public static string SelfLink(this UrlHelper urlHelper, string routeName, string ravenDbIdentifier)
    {
        string[] parts = ravenDbIdentifier.Split('/');
        return urlHelper.Link(routeName, new { id = parts[parts.Length - 1] });
    }
}