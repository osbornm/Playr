using System.Web.Http.Routing;
using Playr.Api;
using Playr.DataModels;

public static class UrlHelperExtensions
{
    public static string LinkToAlbum(this UrlHelper url, DbAlbum album)
    {
        return url.Link(WebApiConfig.Routes.Albums, new { id = album.Id });
    }

    public static string LinkToAlbum(this UrlHelper url, int albumId)
    {
        return url.Link(WebApiConfig.Routes.Albums, new { id = albumId });
    }

    public static string LinkToAlbumDownload(this UrlHelper url, DbAlbum album)
    {
        return url.Link(WebApiConfig.Routes.AlbumDownload, new { id = album.Id });
    }

    public static string LinkToAlbumDownload(this UrlHelper url, int albumId)
    {
        return url.Link(WebApiConfig.Routes.AlbumDownload, new { id = albumId });
    }

    public static string LinkToAlbums(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Routes.Albums, null);
    }

    public static string LinkToAlbumsByArtist(this UrlHelper url, string artist)
    {
        return url.Link(WebApiConfig.Routes.Artists, new { artistName = artist });
    }

    public static string LinkToAlbumsByGenre(this UrlHelper url, string genre)
    {
        return url.Link(WebApiConfig.Routes.Genres, new { genreName = genre });
    }

    public static string LinkToArtistDownload(this UrlHelper url, string artist)
    {
        return url.Link(WebApiConfig.Routes.ArtistDownload, new { artistName = artist });
    }

    public static string LinkToArtists(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Routes.Artists, null);
    }

    public static string LinkToGenres(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Routes.Genres, null);
    }

    public static string LinkToLibrary(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Routes.Library, null);
    }

    public static string LinkToTrackDownload(this UrlHelper url, DbTrack track)
    {
        return url.Link(WebApiConfig.Routes.TrackDownload, new { id = track.Id });
    }

    public static string LinkToTrackDownload(this UrlHelper url, int trackId)
    {
        return url.Link(WebApiConfig.Routes.TrackDownload, new { id = trackId });
    }

    public static string LinkToTracks(this UrlHelper url, DbAlbum album)
    {
        return url.Link(WebApiConfig.Routes.Tracks, new { id = album.Id });
    }

    public static string LinkToTracks(this UrlHelper url, int albumId)
    {
        return url.Link(WebApiConfig.Routes.Tracks, new { id = albumId });
    }
}