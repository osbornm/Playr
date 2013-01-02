using System.Web.Http.Routing;
using Playr.Api;
using Playr.DataModels;

public static class UrlHelperExtensions
{
    public static string LinkToAlbum(this UrlHelper url, DbAlbum album)
    {
        return url.Link(WebApiConfig.Library.Albums, new { id = album.Id });
    }

    public static string LinkToAlbum(this UrlHelper url, int albumId)
    {
        return url.Link(WebApiConfig.Library.Albums, new { id = albumId });
    }

    public static string LinkToAlbumDownload(this UrlHelper url, DbAlbum album)
    {
        return url.Link(WebApiConfig.Library.AlbumDownload, new { id = album.Id });
    }

    public static string LinkToAlbumDownload(this UrlHelper url, int albumId)
    {
        return url.Link(WebApiConfig.Library.AlbumDownload, new { id = albumId });
    }

    public static string LinkToAlbums(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Library.Albums, null);
    }

    public static string LinkToAlbumsByArtist(this UrlHelper url, string artist)
    {
        return url.Link(WebApiConfig.Library.Artists, new { artistName = artist });
    }

    public static string LinkToAlbumsByGenre(this UrlHelper url, string genre)
    {
        return url.Link(WebApiConfig.Library.Genres, new { genreName = genre });
    }

    public static string LinkToArtistDownload(this UrlHelper url, string artist)
    {
        return url.Link(WebApiConfig.Library.ArtistDownload, new { artistName = artist });
    }

    public static string LinkToArtists(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Library.Artists, null);
    }

    public static string LinkToGenres(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Library.Genres, null);
    }

    public static string LinkToLibrary(this UrlHelper url)
    {
        return url.Link(WebApiConfig.Library.Root, null);
    }

    public static string LinkToTrackDownload(this UrlHelper url, DbTrack track)
    {
        return url.Link(WebApiConfig.Library.TrackDownload, new { id = track.Id });
    }

    public static string LinkToTrackDownload(this UrlHelper url, int trackId)
    {
        return url.Link(WebApiConfig.Library.TrackDownload, new { id = trackId });
    }

    public static string LinkToTracks(this UrlHelper url, DbAlbum album)
    {
        return url.Link(WebApiConfig.Library.Tracks, new { id = album.Id });
    }

    public static string LinkToTracks(this UrlHelper url, int albumId)
    {
        return url.Link(WebApiConfig.Library.Tracks, new { id = albumId });
    }
}