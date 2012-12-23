using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using Playr.DataModels;
using Playr.DataModels.Indexes;
using Raven.Client;
using FileMetadata = TagLib.File;

namespace Playr.Services
{
    public class MusicLibraryService
    {
        Lazy<IDocumentSession> session = new Lazy<IDocumentSession>(Database.OpenSession, isThreadSafe: false);

        private IDocumentSession Session
        {
            get { return session.Value; }
        }

        public DbTrack AddFile(string filePath, MediaTypeHeaderValue mediaType)
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

            var normalizedArtistName = Normalize(track.ArtistName);
            var normalizedAlbumName = Normalize(track.AlbumName);
            var normalizedTrackName = Normalize(track.Name);
            var normalizedDiscPrefix = track.DiscNumber > 1 || file.Tag.DiscCount > 1 ? track.DiscNumber.ToString() + "-" : "";

            track.Location = Path.Combine(
                Program.MusicLibraryPath,
                normalizedArtistName,
                normalizedAlbumName,
                String.Format("{0}{1:00} {2}{3}", normalizedDiscPrefix, track.TrackNumber, normalizedTrackName, extension)
            );

            if (File.Exists(track.Location))
            {
                File.Delete(filePath);
                track = Session.Query<DbTrack>().Where(t => t.Location == track.Location).Single();
            }
            else
            {
                Program.EnsurePathExists(Path.GetDirectoryName(track.Location));
                File.Move(filePath, track.Location);

                Session.Store(track);
                Session.SaveChanges();
            }

            return track;
        }

        private T Get<T>(Expression<Func<T, bool>> filter)
        {
            return Session.Query<T>().Where(filter).FirstOrDefault();
        }

        private T Get<T, TIndexCreator>(Expression<Func<T, bool>> filter)
            where TIndexCreator : Raven.Client.Indexes.AbstractIndexCreationTask, new()
        {
            return Session.Query<T, TIndexCreator>().Where(filter).FirstOrDefault();
        }

        public DbAlbum GetAlbum(string artistName, string albumName)
        {
            return Get<DbAlbum, DbAlbum_LowercaseLookup>(a => a.ArtistName == artistName.ToLowerInvariant()
                                                           && a.Name == albumName.ToLowerInvariant());
        }

        public List<DbAlbum> GetAlbums()
        {
            return Session.Query<DbAlbum>().ToList();
        }

        public List<DbTrack> GetTracks()
        {
            return Session.Query<DbTrack>().ToList();
        }

        private DbAlbum GetOrCreateAlbum(string artistName, string albumName, string genre)
        {
            var album = GetAlbum(artistName, albumName);

            if (album == null)
            {
                album = new DbAlbum { ArtistName = artistName, Name = albumName, Genre = genre };
                Session.Store(album);
                Session.SaveChanges();
            }

            return album;
        }

        private string Normalize(string name)
        {
            return name;
        }
    }
}
