using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Playr.DataModels;
using Playr.DataModels.Indexes;
using Raven.Client;
using Raven.Client.Linq;
using FileMetadata = TagLib.File;

namespace Playr.Services
{
    public class MusicLibraryService
    {
        public virtual DbTrack AddFile(string filePath, MediaTypeHeaderValue mediaType)
        {
            var extension = mediaType.ToFileExtension();
            var newPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + extension);

            if (newPath != filePath)
            {
                File.Move(filePath, newPath);
                filePath = newPath;
            }

            var file = FileMetadata.Create(filePath);
            var album = GetOrCreateAlbum(file.Tag.FirstAlbumArtist, file.Tag.Album, file.Tag.FirstGenre);
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
                    extension
                )
            );

            using (var session = Database.OpenSession())
            {
                if (File.Exists(track.Location))
                {
                    File.Delete(filePath);
                    track = session.Query<DbTrack>().Where(t => t.Location == track.Location).Single();
                }
                else
                {
                    PathHelpers.EnsurePathExists(Path.GetDirectoryName(track.Location));
                    File.Move(filePath, track.Location);

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

        // Private helpers

        private DbAlbum GetOrCreateAlbum(string artistName, string albumName, string genre)
        {
            using (var session = Database.OpenSession())
            {
                var album = GetAlbumByArtistAndAlbumName(artistName, albumName, session);

                if (album == null)
                {
                    album = new DbAlbum { ArtistName = artistName, Name = albumName, Genre = genre };
                    session.Store(album);
                    session.SaveChanges();
                }

                return album;
            }
        }
    }
}
