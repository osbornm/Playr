using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Routing;
using Playr.DataModels;

public static class LibraryEndpoints
{
    const string Album = "Album";
    public const string AlbumUrl = "api/library/albums/{id}";
    const string Albums = "Albums";
    public const string AlbumsUrl = "api/library/albums";
    const string AlbumDownload = "DownloadAlbum";
    public const string AlbumDownloadUrl = "api/library/albums/{id}/download";
    const string AlbumArtwork = "AlbumArtwork";
    public const string AlbumArtworkUrl = "api/library/albums/{id}/artwork";
    const string AlbumsByArtist = "AlbumsByArtist";
    public const string AlbumsByArtistUrl = "api/library/artists/{artistName}/albums";
    const string Artist = "Artist";
    public const string ArtistUrl = "api/library/artists/{artistName}";
    const string Artists = "Artists";
    public const string ArtistsUrl = "api/library/artists";
    const string ArtistDownload = "DownloadArtist";
    public const string ArtistDownloadUrl = "api/library/artists/{artistName}/download";
    const string ArtistFanart = "ArtistFanart";
    public const string ArtistFanartUrl = "api/library/artists/{artistName}/fanart/{fanartId}";
    const string Genres = "Genres";
    public const string GenresUrl = "api/library/genres/{genreName}";
    const string Root = "Library";
    public const string RootUrl = "api/library";
    const string Tracks = "Tracks";
    public const string TracksUrl = "api/library/albums/{id}/tracks";
    const string TrackDownload = "TrackDownload";
    public const string TrackDownloadUrl = "api/library/tracks/{id}/download";

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
            routeTemplate: AlbumsUrl,
            defaults: new { controller = "Albums", action = "Albums" }
        );

        config.Routes.MapHttpRoute(
            name: Artists,
            routeTemplate: ArtistsUrl,
            defaults: new { controller = "Artists", action = "GetArtists" }
        );

        config.Routes.MapHttpRoute(
            name: Artist,
            routeTemplate: ArtistUrl,
            defaults: new { controller = "Artists", action = "GetArtist" }
        );

        config.Routes.MapHttpRoute(
            name: AlbumsByArtist,
            routeTemplate: AlbumsByArtist,
            defaults: new { controller = "Artists", action = "GetAlbumsByArtist" }
        );

        config.Routes.MapHttpRoute(
            name: ArtistDownload,
            routeTemplate: ArtistDownloadUrl,
            defaults: new { controller = "Download", action = "Artist" }
        );

        config.Routes.MapHttpRoute(
            name: ArtistFanart,
            routeTemplate: ArtistFanartUrl,
            defaults: new { controller = "Artists", action = "GetFanart" }
        );

        config.Routes.MapHttpRoute(
            name: Genres,
            routeTemplate: GenresUrl,
            defaults: new { controller = "Genres", genreName = RouteParameter.Optional }
        );

        config.Routes.MapHttpRoute(
            name: Tracks,
            routeTemplate: TracksUrl,
            defaults: new { controller = "Tracks" }
        );

        config.Routes.MapHttpRoute(
            name: TrackDownload,
            routeTemplate: TrackDownloadUrl,
            defaults: new { controller = "Download", action = "Track" }
        );
    }
}
public static partial class RouteLinks
{
    public static string LinkToAlbum(DbAlbum album)
    {
        return LinkToAlbum(album.Id);
    }

    public static string LinkToAlbum(int albumId)
    {
        var replacements = new Dictionary<string, string>() { { "{id}", albumId.ToString() } };
        return ReplaceTokens(LibraryEndpoints.AlbumUrl, replacements);
    }

    public static string LinkToAlbumArt(DbAlbum album)
    {
        return LinkToAlbumArt(album.Id);
    }

    public static string LinkToAlbumArt(int albumId)
    {
        var replacements = new Dictionary<string, string>() { { "{id}", albumId.ToString() } };
        return ReplaceTokens(LibraryEndpoints.AlbumArtworkUrl, replacements);
    }

    public static string LinkToAlbumDownload(DbAlbum album)
    {
        return LinkToAlbumDownload(album.Id);
    }

    public static string LinkToAlbumDownload(int albumId)
    {
        var replacements = new Dictionary<string, string>() { { "{id}", albumId.ToString() } };
        return ReplaceTokens(LibraryEndpoints.AlbumDownloadUrl, replacements);
    }

    public static string LinkToAlbums()
    {
        return LibraryEndpoints.AlbumsUrl;
    }

    public static string LinkToAlbumsByArtist(string artist)
    {
        var replacements = new Dictionary<string, string>() { { "{artistName}", artist } };
        return ReplaceTokens(LibraryEndpoints.ArtistUrl, replacements);
    }

    public static string LinkToAlbumsByGenre(string genre)
    {
        var replacements = new Dictionary<string, string>() { { "{genreName}", genre } };
        return ReplaceTokens(LibraryEndpoints.GenresUrl, replacements);
    }

    public static string LinkToArtist(string artist)
    {
        var replacements = new Dictionary<string, string>() { { "{artistName}", artist } };
        return ReplaceTokens(LibraryEndpoints.ArtistUrl, replacements);
    }

    public static string LinkToArtistDownload(string artist)
    {
        var replacements = new Dictionary<string, string>() { { "{artistName}", artist } };
        return ReplaceTokens(LibraryEndpoints.ArtistDownloadUrl, replacements);
    }

    public static string LinkToArtistFanart(string artist, string fanartId)
    {
        var repalcements = new Dictionary<string, string>();
        repalcements.Add("{artistName}", artist);
        repalcements.Add("{fanartId}", fanartId);
        return ReplaceTokens(LibraryEndpoints.ArtistFanartUrl, repalcements);
    }

    public static string LinkToArtists()
    {
        return LibraryEndpoints.ArtistsUrl;
    }

    public static string LinkToGenres()
    {
        var replacements = new Dictionary<string, string>() { { "{genreName}", String.Empty } };
        return ReplaceTokens(LibraryEndpoints.GenresUrl, replacements);
    }

    public static string LinkToLibrary()
    {
        return LibraryEndpoints.RootUrl;
    }

    public static string LinkToTrackDownload(DbTrack track)
    {
        return LinkToTrackDownload(track.Id);
    }

    public static string LinkToTrackDownload(int trackId)
    {
        return Regex.Replace(LibraryEndpoints.TrackDownloadUrl, Regex.Escape("{id}"), Regex.Escape(trackId.ToString()), RegexOptions.IgnoreCase);
    }

    public static string LinkToTracks(DbAlbum album)
    {
        return LinkToTracks(album.Id);
    }

    public static string LinkToTracks(int albumId)
    {
        var replacements = new Dictionary<string, string>() { { "{id}", albumId.ToString() } };
        return ReplaceTokens(LibraryEndpoints.TracksUrl, replacements);
    }

    private static string ReplaceTokens(string route, Dictionary<string, string> replacementTokens)
    {
        route = "/" + route;
        foreach (var token in replacementTokens)
        {
            route = Regex.Replace(route, Regex.Escape(token.Key), Regex.Escape(token.Value), RegexOptions.IgnoreCase);
        }
        return route;
    }
}