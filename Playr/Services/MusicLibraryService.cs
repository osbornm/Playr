using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Playr.DataModels;
using Playr.DataModels.Indexes;
using Raven.Client;
using Raven.Client.Linq;
using FileMetadata = TagLib.File;

namespace Playr.Services
{
    public class MusicLibraryService
    {
        public virtual DbTrack AddFile(string fileName)
        {
            var file = FileMetadata.Create(fileName);
            var album = GetOrCreateAlbum(file);
            var track = new DbTrack
            {
                AlbumId = album.Id,
                AlbumName = file.Tag.Album,
                ArtistName = file.Tag.FirstPerformer,
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
                if (File.Exists(track.Location))
                {
                    File.Delete(fileName);
                    track = session.Query<DbTrack>().Where(t => t.Location == track.Location).Single();
                }
                else
                {
                    PathHelpers.EnsurePathExists(Path.GetDirectoryName(track.Location));
                    File.Move(fileName, track.Location);

                    session.Store(track);
                    session.SaveChanges();
                }
            }

            return track;
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
                return session.Query<DbTrack>().Customize(x => x.RandomOrdering()).FirstOrDefault();
        }

        // Private helpers

        private static object lockObject = new object();

        private DbAlbum GetOrCreateAlbum(FileMetadata file)
        {
            var artistName = file.Tag.FirstAlbumArtist;
            var albumName =file.Tag.Album;
            var genre = file.Tag.FirstGenre;
            DbAlbum album;

            // TODO: This is a terrible way to prevent re-entry, but it'll be good enough for now
            // until we can make a worker-thread to do background processing on uploads.
            lock (lockObject)
            {
                using (var session = Database.OpenSession())
                {
                    album = GetAlbumByArtistAndAlbumName(artistName, albumName, session);
                    if (album == null)
                    {
                        album = new DbAlbum { ArtistName = artistName, Name = albumName, Genre = genre };
                        session.Store(album);
                        session.SaveChanges();
                    }
                }

                var albumArtwork = Path.Combine(Program.AlbumArtworkPath, String.Format("{0}.jpg", album.Id));
                if (!File.Exists(albumArtwork))
                {

                    var img = file.Tag.Pictures.FirstOrDefault();
                    if (img != null)
                    {
                        File.WriteAllBytes(albumArtwork, img.Data.ToArray());
                    }
                }
            }
            // TODO: Consider FanArt here too?
            return album;
        }
    }
}
