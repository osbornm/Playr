using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using iTunesLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Playr.Api.Models;
using Raven.Client;
using Raven.Client.Embedded;
using SpeechLib;
using System.Collections.Concurrent;
using Raven.Database.Server;

namespace Playr.Api
{
    public static class Helpers
    {
        public static string _artworkUrlFormatString = ApplicationSettings.apiBaseUrl + "/songs/{0}/artwork";
        public static string _downloadUrlFormatString = ApplicationSettings.apiBaseUrl + "/songs/{0}/download";
        public static string _albumDownloadUrlFormatString = ApplicationSettings.apiBaseUrl + "/albums/{0}/download";

        static ConcurrentDictionary<int, Tuple<int, int>> _lookup = new ConcurrentDictionary<int, Tuple<int, int>>();

        public static void InitializeTrackCache(this iTunesAppClass itunes)
        {
            foreach (IITTrack t in itunes.LibraryPlaylist.Tracks)
            {
                _lookup.GetOrAdd(t.TrackDatabaseID, _ =>
                    new Tuple<int, int>(
                        itunes.ITObjectPersistentIDHigh[t],
                        itunes.ITObjectPersistentIDLow[t]
                    )
                );
            }
        }

        public static string GetToken(this HttpRequestMessage request)
        {
            var header = request.Headers.SingleOrDefault(x => x.Key == "x-playr-token");
            if (header.Value == null)
            {
                return String.Empty;
            }
            return header.Value.FirstOrDefault();
        }

        public static bool IsFanArtEnabled()
        {
            return !String.IsNullOrEmpty(ApplicationSettings.fanArtApiKey);
        }

        public static IITTrack GetTrackById(this iTunesAppClass itunes, int id)
        {
            Tuple<int, int> persistentIds;
            if (!_lookup.TryGetValue(id, out persistentIds))
                return null;

            var track = itunes.LibraryPlaylist.Tracks.get_ItemByPersistentID(persistentIds.Item1, persistentIds.Item2);
            if (track != null)
                return (IITTrack)track;

            return null;
        }

        public static List<IITFileOrCDTrack> GetAlbumTracks(this iTunesAppClass itunes, string album)
        {
            var tracks = new List<IITFileOrCDTrack>();
            foreach (IITTrack t in itunes.LibraryPlaylist.Tracks)
            {
                if (t.Album != null && t.Album.Equals(album, StringComparison.InvariantCultureIgnoreCase))
                {
                    tracks.Add((IITFileOrCDTrack)t);
                }
            }
            return tracks;
        }

        public static Song toSong(this IITTrack t, iTunesAppClass itunes)
        {
            _lookup.GetOrAdd(t.TrackDatabaseID, _ =>
                new Tuple<int, int>(
                    itunes.ITObjectPersistentIDHigh[t],
                    itunes.ITObjectPersistentIDLow[t]
                )
            );

            return new Song
            {
                Id = t.TrackDatabaseID,
                Album = t.Album,
                Artist = t.Artist,
                Rating = t.Rating,
                Title = t.Name,
                ArtworkUrl = String.Format(_artworkUrlFormatString, t.TrackDatabaseID),
                DownloadUrl = String.Format(_downloadUrlFormatString, t.TrackDatabaseID),
                AlbumDownloadUrl = String.Format(_albumDownloadUrlFormatString, t.TrackDatabaseID),
                Duration = t.Duration
            };
        }

        public static Song toSong(this IITTrack t, int position, iTunesAppClass itunes)
        {
            var song = Helpers.toSong(t, itunes);
            song.Poisition = position;
            return song;
        }

        public static Song toSong(this IITTrack t, User u, iTunesAppClass itunes)
        {
            var isFavorite = u != null && u.Favorites.Where(s => s.Id == t.TrackDatabaseID).Any();
            var song = Helpers.toSong(t, itunes);
            song.IsFavorite = isFavorite;
            return song;
        }

        public static Song toSong(this IITTrack t, int position, User u, iTunesAppClass itunes)
        {
            var song = Helpers.toSong(t, u, itunes);
            song.Poisition = position;
            return song;
        }

        private static IDocumentStore docStore;

        public static IDocumentStore DocumentStore
        {
            get
            {
                if (docStore == null)
                    throw new InvalidOperationException("IDocumentStore has not been initialized.");
                return docStore;
            }
        }

        public static void InitializeDocumentStore()
        {
            docStore = new EmbeddableDocumentStore()
            {
                UseEmbeddedHttpServer = true
            };
            docStore.Initialize();
        }

        public static void Speak(this iTunesApp itunes, string message)
        {
            var pasueMusic = itunes.PlayerState == ITPlayerState.ITPlayerStatePlaying;
            if (pasueMusic)
            {
                itunes.Pause();
            }
            var voice = new SpVoice();
            voice.Speak(message, SpeechVoiceSpeakFlags.SVSFDefault);
            if (pasueMusic)
            {
                itunes.Play();
            }
        }
    }

    public static class HttpContentExtensions
    {
        public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            string pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
            }

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                    (copyTask) =>
                    {
                        fileStream.Close();
                    });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                throw;
            }
        }
    }

}
