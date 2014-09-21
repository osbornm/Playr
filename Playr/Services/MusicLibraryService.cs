using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using Nancy.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Playr.DataModels;
using Playr.DataModels.Indexes;
using Raven.Client;
using Raven.Client.Linq;
using FileMetadata = TagLib.File;

namespace Playr.Services
{
    public class MusicLibraryService
    {
        private const string UNKNOWN_ALBUM = "Unknown Album";
        private const string UNKNOWN_ARTIST = "Unknown Artist";

        public virtual DbTrack AddFile(string fileName)
        {
            var file = FileMetadata.Create(fileName);
            var album = GetOrCreateAlbum(file);
            var track = new DbTrack
            {
                AlbumId = album.Id,
                AlbumName = file.Tag.Album ?? UNKNOWN_ALBUM,
                ArtistName = file.Tag.FirstPerformer ?? file.Tag.FirstAlbumArtist ?? UNKNOWN_ARTIST,
                AudioBitrate = file.Properties.AudioBitrate,
                AudioChannels = file.Properties.AudioChannels,
                AudioSampleRate = file.Properties.AudioSampleRate,
                BeatsPerMinute = file.Tag.BeatsPerMinute,
                Composer = file.Tag.FirstComposer,
                Conductor = file.Tag.Conductor,
                DiscNumber = file.Tag.Disc,
                Name = file.Tag.Title,
                Time = file.Properties.Duration,
                TrackNumber = file.Tag.Track,
                Year = file.Tag.Year
            };

            track.Location = Path.Combine(
                Program.MusicLibraryPath,
                PathHelpers.ToFolderName(track.ArtistName),
                PathHelpers.ToFolderName(track.AlbumName),
                String.Format(
                    "{0}{1:00} {2}{3}",
                    track.DiscNumber > 1 || file.Tag.DiscCount > 1 ? track.DiscNumber + "-" : "",
                    track.TrackNumber,
                    PathHelpers.ToFileName(track.Name),
                    Path.GetExtension(fileName)
                )
            );

            using (var session = Database.OpenSession())
            {
                if (track.Location != fileName && File.Exists(track.Location))
                {
                    Log.Debug("Replacing existing track {0}", track.Location);
                    File.Delete(fileName);
                    track = session.Query<DbTrack>().Where(t => t.Location == track.Location).Single();
                }
                else
                {
                    if (track.Location != fileName)
                    {
                        Log.Debug("Moving {0} to {1}", fileName, track.Location);
                        PathHelpers.EnsurePathExists(Path.GetDirectoryName(track.Location));
                        File.Move(fileName, track.Location);
                    }

                    session.Store(track);
                    session.SaveChanges();
                }
            }

            return track;
        }

        public void ProcessFile(string fileName)
        {
            Log.Info("Processing: {0}", fileName);

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".zip":
                    ProcessFile_Zip(fileName);
                    break;

                case ".mp3":
                case ".m4a":
                    ProcessFile_Audio(fileName);
                    break;

                default:
                    Log.Warning("Unsupported file type: {0}", Path.GetFileName(fileName));
                    File.Delete(fileName);
                    break;
            }
        }

        public void ProcessFile(string fileName, MediaTypeHeaderValue mediaType)
        {
            var extension = mediaType.ToFileExtension();
            var newPath = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + extension);

            if (newPath != fileName)
            {
                File.Move(fileName, newPath);
                fileName = newPath;
            }

            ProcessFile(fileName);
        }

        private void ProcessFile_Audio(string fileName)
        {
            try
            {
                AddFile(fileName);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        private void ProcessFile_Zip(string zipFile)
        {
            try
            {
                var outputPath = Path.Combine(Program.TempPath, Guid.NewGuid().ToString("N"));
                PathHelpers.EnsurePathExists(outputPath, forceClean: true);

                using (var zip = ZipFile.Read(zipFile))
                    zip.ExtractSelectedEntries("name = *.m4a or name = *.mp3 or name = *.zip", null, outputPath);

                ProcessFolder(outputPath);

                SwallowExceptions(() => File.Delete(zipFile));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void ProcessFolder(string folder)
        {
            Log.Info("Searching: {0}", folder);

            foreach (var file in Directory.GetFiles(folder))
                ThreadPool.QueueUserWorkItem(_ => ProcessFile(file));

            foreach (var subFolder in Directory.GetDirectories(folder))
                ProcessFolder(subFolder);
        }

        private void SwallowExceptions(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }

        public DbTrack QueueOnly(int id)
        {
            DbTrack track;
            using (var session = Database.OpenSession())
            {
                track = session.Load<DbTrack>(id);
                track.QueueOnly = true;
                session.SaveChanges();
            }

            DbTrack bar;
            using (var session = Database.OpenSession())
            {
                bar = session.Load<DbTrack>(id);
            }
            return bar;
        }

        public virtual DbAlbum GetAlbumById(int id)
        {
            DbAlbum album = null;

            using (var session = Database.OpenSession())
            {
                var tracks = session.Query<DbTrack, DbTrack_ByAlbumId>()
                                    .Where(track => track.AlbumId == id)
                                    .ToArray();

                if (tracks.Length > 0)
                {
                    album = session.Load<DbAlbum>(id);
                    album.Tracks = tracks;
                }
            }

            return album;
        }

        public virtual DbAlbum GetAlbumByArtistAndAlbumName(string artistName, string albumName)
        {
            using (var session = Database.OpenSession())
                return GetAlbumByArtistAndAlbumName(artistName, albumName, session);
        }

        private DbAlbum GetAlbumByArtistAndAlbumName(string artistName, string albumName, IDocumentSession session)
        {
            return session.Query<DbAlbum, DbAlbum_LowercaseLookup>()
                          .Where(a => a.ArtistName == artistName.ToLowerInvariant()
                                   && a.Name == albumName.ToLowerInvariant())
                          .Customize(x => x.WaitForNonStaleResults())
                          .FirstOrDefault();
        }

        public virtual List<DbAlbum> GetAlbums()
        {
            using (var session = Database.OpenSession())
                return session.Query<DbAlbum>().ToList();
        }

        public virtual List<DbAlbum> GetAlbumsByArtist(string artistName)
        {
            using (var session = Database.OpenSession())
                return session.Query<DbAlbum, DbAlbum_ByArtist>().Where(album => album.ArtistName == artistName.ToLowerInvariant()).ToList();
        }

        public virtual List<DbAlbum> GetAlbumsByGenre(string genre)
        {
            using (var session = Database.OpenSession())
                return session.Query<DbAlbum, DbAlbum_ByGenre>().Where(album => album.Genre == genre.ToLowerInvariant()).ToList();
        }

        public virtual List<string> GetArtists()
        {
            using (var session = Database.OpenSession())
                return session.Query<DbAlbum, DbAlbum_Artists>().As<DbAlbum_Artists.Result>().Select(result => result.ArtistName).ToList();
        }

        public virtual List<string> GetGenres()
        {
            using (var session = Database.OpenSession())
                return session.Query<DbAlbum, DbAlbum_Genres>().As<DbAlbum_Genres.Result>().Select(result => result.Genre).ToList();
        }

        public virtual DbLibrary GetLibraryInfo()
        {
            using (var session = Database.OpenSession())
            {
                return new DbLibrary
                {
                    TotalAlbums = session.Query<DbAlbum>().Count(),
                    TotalTracks = session.Query<DbTrack>().Count()
                };
            }
        }

        public virtual DbTrack GetTrackById(int id)
        {
            using (var session = Database.OpenSession())
                return session.Load<DbTrack>(id);
        }

        public virtual List<DbTrack> GetTracks(int albumId)
        {
            using (var session = Database.OpenSession())
                return session.Query<DbTrack>().Where(track => track.AlbumId == albumId).ToList();
        }

        public virtual DbTrack GetRandomTrack()
        {
            using (var session = Database.OpenSession())
                return session.Query<DbTrack>().Where(t => !t.QueueOnly).Customize(x => x.RandomOrdering()).FirstOrDefault();
        }

        public virtual int TotalTrackCount()
        {
            using (var session = Database.OpenSession())
                return session.Query<DbTrack>().Count();
        }

        // Private helpers

        private static readonly object databaseLock = new object();
        private static readonly object albumartLock = new object();
        private static readonly object fanartLock = new object();

        private DbAlbum GetOrCreateAlbum(FileMetadata file)
        {
            var artistName = file.Tag.FirstAlbumArtist ?? UNKNOWN_ARTIST;
            var albumName = file.Tag.Album ?? UNKNOWN_ALBUM;
            var genre = file.Tag.FirstGenre;
            DbAlbum album;

            // TODO: This is a terrible way to prevent re-entry, but it'll be good enough for now
            // until we can make a worker-thread to do background processing on uploads.
            using (var session = Database.OpenSession())
            {
                album = GetAlbumByArtistAndAlbumName(artistName, albumName, session);
                if (album == null)
                {
                    lock (databaseLock)
                    {
                        album = GetAlbumByArtistAndAlbumName(artistName, albumName, session);
                        if (album == null)
                        {
                            album = new DbAlbum { ArtistName = artistName, Name = albumName, Genre = genre };
                            session.Store(album);
                            session.SaveChanges();
                        }
                    }
                }
            }

            var albumArtwork = Path.Combine(Program.AlbumArtworkPath, String.Format("{0}.jpg", album.Id));
            if (!File.Exists(albumArtwork))
            {
                lock (albumartLock)
                {
                    if (!File.Exists(albumArtwork))
                    {
                        var img = file.Tag.Pictures.FirstOrDefault();
                        if (img != null)
                        {
                            File.WriteAllBytes(albumArtwork, img.Data.ToArray());
                        }
                    }
                }
            }

            if (Program.FanartEnabled)
            {
                var fanartPath = Path.Combine(Program.FanArtworkPath, PathHelpers.ToFolderName(album.ArtistName));
                if (!Directory.Exists(fanartPath))
                {
                    lock (fanartLock)
                    {
                        if (!Directory.Exists(fanartPath))
                        {
                            Directory.CreateDirectory(fanartPath);
                            var artUrls = GetUrlsForArtist(album.ArtistName).Result;
                            DownloadUrls(artUrls, fanartPath).Wait();
                        }
                    }
                }
            }

            return album;
        }

        private Task DownloadUrls(List<string> urls, string outputFolder)
        {
            var tasks = new List<Task>();

            foreach (var url in urls)
            {
                tasks.Add(DownloadUrl(url, Path.Combine(outputFolder, Guid.NewGuid().ToString("n") + ".jpg")));
            }

            return Task.WhenAll(tasks);
        }

        private async Task DownloadUrl(string url, string outputPath)
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                Log.Message("Downloading {0} to {1}", url, outputPath);
                await response.Content.ReadAsFileAsync(outputPath);
            }
        }

        private async Task<List<string>> GetUrlsForArtist(string artist)
        {
            var fanartUrls = new List<string>();

            if (artist != UNKNOWN_ARTIST)
            {
                // Search for the MID first
                var httpClient = new HttpClient();
                var musicbrainzUrl = String.Format("http://search.musicbrainz.org/ws/2/artist/?query={0}&fmt=json", HttpUtility.UrlEncode(artist));
                var musicbrainzResponse = await httpClient.GetAsync(musicbrainzUrl);
                var searchResult = await musicbrainzResponse.Content.ReadAsAsync<SearchResult>();

                if (searchResult.artist_list != null && searchResult.artist_list.artist != null && searchResult.artist_list.artist.Count > 0)
                {
                    var mid = searchResult.artist_list.artist.OrderByDescending(a => a.score).First().id;
                    if (mid != null)
                    {
                        // Once there is an ID get the FanArt
                        var url = String.Format("http://webservice.fanart.tv/v3/music/{0}?api_key={1}", HttpUtility.UrlEncode(mid), HttpUtility.UrlEncode(Program.FanartApiKey));
                        try
                        {
                            var response = await httpClient.GetStringAsync(url);
                            if (response != "null")
                            {
                                var artistJson = JObject.Parse(response);

                                if (artistJson != null && artistJson.HasValues)
                                {
                                    var bgs = artistJson["artistbackground"];
                                    foreach (var background in bgs)
                                    {
                                        fanartUrls.Add(background["url"].ToString());
                                    }
                                }
                            }
                        }
                        catch { } // the fan art service sometimes has malformed JSON and can cause errors.
                    }
                }
            }

            return fanartUrls;
        }

        // Mapping classes to MusicBrainz JSON

        private class SearchResult
        {
            public string created { get; set; }

            [JsonProperty("artist-list")]
            public artistResults artist_list { get; set; }
        }

        private class artistResults
        {
            public List<artist> artist { get; set; }

        }

        private class artist
        {
            public int score { get; set; }
            public string id { get; set; }
        }
    }
}
