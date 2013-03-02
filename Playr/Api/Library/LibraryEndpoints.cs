using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Routing;
using Playr.DataModels;

public static class LibraryEndpoints
{
    const string Album = "Album";
    const string AlbumUrl = "api/library/albums/{id}";
    const string Albums = "Albums";
    const string AlbumDownload = "DownloadAlbum";
    const string AlbumDownloadUrl = "api/library/albums/{id}/download";
    const string AlbumArtwork = "AlbumArtwork";
    const string AlbumArtworkUrl = "api/library/albums/{id}/artwork";
    const string AlbumsByArtist = "AlbumsByArtist";
    const string Artist = "Artist";
    const string Artists = "Artists";
    const string ArtistDownload = "DownloadArtist";
    const string ArtistFanart = "ArtistFanart";
    const string ArtistFanartUrl = "api/library/artists/{artistName}/fanart/{fanartId}";
    const string Genres = "Genres";
    const string Root = "Library";
    const string RootUrl = "api/library";
    const string Tracks = "Tracks";
    const string TrackDownload = "TrackDownload";
    const string TrackDownloadUrl = "api/library/tracks/{id}/download";



    public static void Configure(HttpConfiguration config)
    {
        config.Routes.MapHttpRoute(
            name: Root,
            routeTemplate: RootUrl,
            defaults: new { controller = "Library" }
        );

        config.Routes.MapHttpRoute(
            name: Album,
            routeTemplate: AlbumUrl,
            defaults: new { controller = "Albums", action = "Album" }
        );

        config.Routes.MapHttpRoute(
            name: AlbumDownload,
            routeTemplate: AlbumDownloadUrl,
            defaults: new { controller = "Download", action = "Album" }
        );

        config.Routes.MapHttpRoute(
            name: AlbumArtwork,
            routeTemplate: AlbumArtworkUrl,
            defaults: new { controller = "Albums", action = "Artwork" }
        );

        config.Routes.MapHttpRoute(
            name: Albums,
            routeTemplate: "api/library/albums",
            defaults: new { controller = "Albums", action = "Albums" }
        );

        config.Routes.MapHttpRoute(
            name: Artists,
            routeTemplate: "api/library/artists",
            defaults: new { controller = "Artists", action = "GetArtists" }
        );

        config.Routes.MapHttpRoute(
            name: Artist,
            routeTemplate: "api/library/artists/{artistName}",
            defaults: new { controller = "Artists", action = "GetArtist" }
        );

        config.Routes.MapHttpRoute(
            name: AlbumsByArtist,
            routeTemplate: "api/library/artists/{artistName}/albums",
            defaults: new { controller = "Artists", action = "GetAlbumsByArtist" }
        );

        config.Routes.MapHttpRoute(
            name: ArtistDownload,
            routeTemplate: "api/library/artists/{artistName}/download",
            defaults: new { controller = "Download", action = "Artist" }
        );

        config.Routes.MapHttpRoute(
            name: ArtistFanart,
            routeTemplate: ArtistFanartUrl,
            defaults: new { controller = "Artists", action = "GetFanart" }
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
            routeTemplate: TrackDownloadUrl,
            defaults: new { controller = "Download", action = "Track" }
        );
    }


    public static string LinkToAlbum(DbAlbum album)
    {
        return LinkToAlbum(album.Id);
    }

    public static string LinkToAlbum(int albumId)
    {
        return Regex.Replace(AlbumUrl, Regex.Escape("{id}"), Regex.Escape(albumId.ToString()), RegexOptions.IgnoreCase);
    }

    public static string LinkToAlbumArt( DbAlbum album)
    {
        return LinkToAlbumArt(album.Id);
    }

    public static string LinkToAlbumArt(int albumId)
    {
        return Regex.Replace(AlbumArtworkUrl, Regex.Escape("{id}"), Regex.Escape(albumId.ToString()), RegexOptions.IgnoreCase);
    }

    public static string LinkToAlbumDownload(DbAlbum album)
    {
        return LinkToAlbumDownload(album.Id);
    }

    public static string LinkToAlbumDownload(int albumId)
    {
        return Regex.Replace(AlbumDownloadUrl, Regex.Escape("{id}"), Regex.Escape(albumId.ToString()), RegexOptions.IgnoreCase);
    }

    public static string LinkToAlbums(this UrlHelper url)
    {
        return url.Link(Albums, null);
    }

    public static string LinkToAlbumsByArtist(this UrlHelper url, string artist)
    {
        return url.Link(AlbumsByArtist, new { artistName = artist });
    }

    public static string LinkToAlbumsByGenre(this UrlHelper url, string genre)
    {
        return url.Link(Genres, new { genreName = genre });
    }

    public static string LinkToArtist(this UrlHelper url, string artist)
    {
        return url.Link(Artist, new { artistName = artist });
    }

    public static string LinkToArtistDownload(this UrlHelper url, string artist)
    {
        return url.Link(ArtistDownload, new { artistName = artist });
    }

    public static string LinkToArtistFanart(string artist, string fanartId)
    {
        var url = Regex.Replace(ArtistFanartUrl, Regex.Escape("{artistName}"), Regex.Escape(artist), RegexOptions.IgnoreCase);
        return Regex.Replace(url, Regex.Escape("{fanartId}"), Regex.Escape(fanartId), RegexOptions.IgnoreCase);
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

    public static string LinkToTrackDownload(DbTrack track)
    {
        return LinkToTrackDownload(track.Id);
    }

    public static string LinkToTrackDownload(int trackId)
    {
        return Regex.Replace(TrackDownloadUrl, Regex.Escape("{id}"), Regex.Escape(trackId.ToString()), RegexOptions.IgnoreCase);
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