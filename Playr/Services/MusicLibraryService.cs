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
                PathHelpers.ToFileName(String.Format(
                    "{0}{1:00} {2}{3}",
                    track.DiscNumber > 1 || file.Tag.DiscCount > 1 ? track.DiscNumber + "-" : "",
                    track.TrackNumber,
                    track.Name,
                    extension
                ))
            );

            if (File.Exists(track.Location))
            {
                File.Delete(filePath);
                track = Session.Query<DbTrack>().Where(t => t.Location == track.Location).Single();
            }
            else
            {
                PathHelpers.EnsurePathExists(Path.GetDirectoryName(track.Location));
                File.Move(filePath, track.Location);

                Session.Store(track);
                Session.SaveChanges();
            }

            return track;
        }

        public virtual DbAlbum GetAlbumById(int id)
        {
            return Session.Load<DbAlbum>("DbAlbums/" + id);
        }

        public virtual DbAlbum GetAlbumByArtistAndAlbumName(string artistName, string albumName)
        {
            return Get<DbAlbum, DbAlbum_LowercaseLookup>(a => a.ArtistName == artistName.ToLowerInvariant()
                                                           && a.Name == albumName.ToLowerInvariant());
        }

        public virtual List<DbAlbum> GetAlbums()
        {
            return Session.Query<DbAlbum>().ToList();
        }

        public virtual List<DbTrack> GetTracks()
        {
            return Session.Query<DbTrack>().ToList();
        }

        public virtual DbTrack GetTrackById(int id)
        {
            return Session.Load<DbTrack>("DbTracks/" + id);
        }

        // Private helpers

        private T Get<T>(Expression<Func<T, bool>> filter)
        {
            return Session.Query<T>().Where(filter).FirstOrDefault();
        }

        private T Get<T, TIndexCreator>(Expression<Func<T, bool>> filter)
            where TIndexCreator : Raven.Client.Indexes.AbstractIndexCreationTask, new()
        {
            return Session.Query<T, TIndexCreator>().Where(filter).FirstOrDefault();
        }

        private DbAlbum GetOrCreateAlbum(string artistName, string albumName, string genre)
        {
            var album = GetAlbumByArtistAndAlbumName(artistName, albumName);

            if (album == null)
            {
                album = new DbAlbum { ArtistName = artistName, Name = albumName, Genre = genre };
                Session.Store(album);
                Session.SaveChanges();
            }

            return album;
        }
    }
}
