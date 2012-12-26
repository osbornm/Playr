using System.Collections.Generic;
using System.Web.Http.Routing;
using Playr.Api;
using Playr.DataModels;

public static class UrlHelperExtensions
{
    public static string LinkToAlbum(this UrlHelper url, DbAlbum album)
    {
        return Link(url, WebApiConfig.Routes.Albums, album.Id);
    }

    public static string LinkToAlbum(this UrlHelper url, int albumId)
    {
        return Link(url, WebApiConfig.Routes.Albums, albumId);
    }

    public static string LinkToAlbums(this UrlHelper url)
    {
        return Link(url, WebApiConfig.Routes.Albums);
    }

    public static string LinkToAlbumsByGenre(this UrlHelper url, string genre)
    {
        return Link(url, WebApiConfig.Routes.Genres, genre);
    }

    public static string LinkToGenres(this UrlHelper url)
    {
        return Link(url, WebApiConfig.Routes.Genres);
    }

    public static string LinkToLibrary(this UrlHelper url)
    {
        return Link(url, WebApiConfig.Routes.Library);
    }

    public static string LinkToTracks(this UrlHelper url, DbAlbum album)
    {
        return Link(url, WebApiConfig.Routes.Tracks, album.Id);
    }

    public static string LinkToTracks(this UrlHelper url, int albumId)
    {
        return Link(url, WebApiConfig.Routes.Tracks, albumId);
    }

    private static string Link(UrlHelper url, string routeName, object id = null)
    {
        var routeValues = new Dictionary<string, object>();
        if (id != null)
            routeValues.Add("id", id);

        return url.Link(routeName, routeValues);
    }
}