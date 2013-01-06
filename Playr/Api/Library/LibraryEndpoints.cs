using System.Web.Http;
using System.Web.Http.Routing;
using Playr.DataModels;

public static class LibraryEndpoints
{
    const string Albums = "Albums";
    const string AlbumDownload = "DownloadAlbum";
    const string Artists = "Artists";
    const string ArtistDownload = "DownloadArtist";
    const string Genres = "Genres";
    const string Root = "Library";
    const string Tracks = "Tracks";
    const string TrackDownload = "TrackDownload";

    public static void Configure(HttpConfiguration config)
    {
        config.Routes.MapHttpRoute(
            name: Root,
            routeTemplate: "api/library",
            defaults: new { controller = "Library" }
        );

        config.Routes.MapHttpRoute(
            name: Albums,
            routeTemplate: "api/library/albums/{id}",
            defaults: new { controller = "Albums", id = RouteParameter.Optional }
        );

        config.Routes.MapHttpRoute(
            name: AlbumDownload,
            routeTemplate: "api/library/albums/{id}/download",
            defaults: new { controller = "Download", action = "Album" }
        );

        config.Routes.MapHttpRoute(
            name: Artists,
            routeTemplate: "api/library/artists/{artistName}",
            defaults: new { controller = "Artists", artistName = RouteParameter.Optional }
        );

        config.Routes.MapHttpRoute(
            name: ArtistDownload,
            routeTemplate: "api/library/artists/{artistName}/download",
            defaults: new { controller = "Download", action = "Artist" }
        );

        config.Routes.MapHttpRoute(
            name: Genres,
            routeTemplate: "api/library/genres/{genreName}",
            defaults: new { controller = "Genres", genreName = RouteParameter.Optional }
        );

        config.Routes.MapHttpRoute(
            name: Tracks,
            routeTemplate: "api/library/albums/{id}/tracks",
            defaults: new { controller = "Tracks" }
        );

        config.Routes.MapHttpRoute(
            name: TrackDownload,
            routeTemplate: "api/library/tracks/{id}/download",
            defaults: new { controller = "Download", action = "Track" }
        );
    }

    public static string LinkToAlbum(this UrlHelper url, DbAlbum album)
    {
        return url.Link(Albums, new { id = album.Id });
    }

    public static string LinkToAlbum(this UrlHelper url, int albumId)
    {
        return url.Link(Albums, new { id = albumId });
    }

    public static string LinkToAlbumDownload(this UrlHelper url, DbAlbum album)
    {
        return url.Link(AlbumDownload, new { id = album.Id });
    }

    public static string LinkToAlbumDownload(this UrlHelper url, int albumId)
    {
        return url.Link(AlbumDownload, new { id = albumId });
    }

    public static string LinkToAlbums(this UrlHelper url)
    {
        return url.Link(Albums, null);
    }

    public static string LinkToAlbumsByArtist(this UrlHelper url, string artist)
    {
        return url.Link(Artists, new { artistName = artist });
    }

    public static string LinkToAlbumsByGenre(this UrlHelper url, string genre)
    {
        return url.Link(Genres, new { genreName = genre });
    }

    public static string LinkToArtistDownload(this UrlHelper url, string artist)
    {
        return url.Link(ArtistDownload, new { artistName = artist });
    }

    public static string LinkToArtists(this UrlHelper url)
    {
        return url.Link(Artists, null);
    }

    public static string LinkToGenres(this UrlHelper url)
    {
        return url.Link(Genres, null);
    }

    public static string LinkToLibrary(this UrlHelper url)
    {
        return url.Link(Root, null);
    }

    public static string LinkToTrackDownload(this UrlHelper url, DbTrack track)
    {
        return url.Link(TrackDownload, new { id = track.Id });
    }

    public static string LinkToTrackDownload(this UrlHelper url, int trackId)
    {
        return url.Link(TrackDownload, new { id = trackId });
    }

    public static string LinkToTracks(this UrlHelper url, DbAlbum album)
    {
        return url.Link(Tracks, new { id = album.Id });
    }

    public static string LinkToTracks(this UrlHelper url, int albumId)
    {
        return url.Link(Tracks, new { id = albumId });
    }
}