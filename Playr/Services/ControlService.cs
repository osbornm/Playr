using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Playr.DataModels;
using Playr.Helpers;
using Playr.Models;

namespace Playr.Services
{
    public class ControlService : IDisposable
    {
        private AudioService audio;
        private MusicLibraryService library = new MusicLibraryService();
        private Deque<DbTrack> playlist = new Deque<DbTrack>();
        private Deque<DbTrack> previous = new Deque<DbTrack>();
        private int queueLength;

        public ControlService(AudioService audio)
        {
            this.audio = audio;
            audio.PlaybackStopped += Next;

            queueLength = Convert.ToInt32(ConfigurationManager.AppSettings["Playr:QueueLength"]);
            for (int i = 0; i < queueLength; i++)
            {
                var track = library.GetRandomTrack();
                if (track == null)
                {
                    return;
                }
                playlist.AddLast(track);
            }
        }

        public void Spin()
        {
            Next();
        }

        public DbAlbum CurrentAlbum { get; private set; }

        public DbTrack CurrentTrack { get; private set; }

        public TimeSpan CurrentTime
        {
            get
            {
                return audio.CurrentTime;
            }
        }

        public TrackState AudioState
        {
            get
            {
                return audio.AudioState;
            }
        }

        public IEnumerable<DbTrack> Upcoming
        {
            get
            {
                return playlist.ToArray();
            }
        }

        public IEnumerable<DbTrack> Recent
        {
            get
            {
                return previous.ToArray();
            }
        }

        public event Action<DbTrack> CurrentTrackChanged;
        public event Action QueueChanged;
        public event Action Paused;
        public event Action Resumed;
        public event Action Disposed;

        public void Pause()
        {
            audio.Pause();
            OnPause();
        }

        public void Resume()
        {
            audio.Resume();
            OnResume();
        }

        public void Next()
        {
            // Get the song
            if (CurrentTrack != null)
                AddToPrevious(CurrentTrack);

            //if there is nothing in the playlist move on
            if (playlist.Count == 0)
            {
                return;
            }

            CurrentTrack = playlist.PopFirst();
            CurrentAlbum = library.GetAlbumById(CurrentTrack.AlbumId);

            // Fill back up the queue
            var songsToAdd = queueLength - playlist.Count;
            for (int i = 0; i < songsToAdd; i++)
            {
                playlist.AddLast(library.GetRandomTrack());
            }

            // DJ spin that shit
            audio.Play(CurrentTrack.Location);

            OnQueueChanged();
            OnCurrentTrackChanged();
        }

        public void Previous()
        {
            if (previous.Count == 0)
                return;

            playlist.AddFirst(CurrentTrack);
            CurrentTrack = previous.PopLast();
            // DJ spin that shit
            audio.Play(CurrentTrack.Location);

            OnQueueChanged();
            OnCurrentTrackChanged();
        }

        private void AddToPrevious(DbTrack track)
        {
            for (int i = 0; i <= previous.Count - queueLength; i++)
            {
                previous.RemoveFirst();
            }
            previous.AddLast(track);
        }

        private void OnCurrentTrackChanged()
        {
            if (CurrentTrackChanged != null)
                CurrentTrackChanged(CurrentTrack);
        }

        private void OnQueueChanged()
        {
            if (QueueChanged != null)
                QueueChanged();
        }

        private void OnPause()
        {
            if (Paused != null)
                Paused();
        }

        private void OnResume()
        {
            if (Resumed != null)
                Resumed();
        }

        //TODO: QueueSong, PlayNow

        public void Dispose()
        {
            //TODO: Save Queue to RavenDB for Resume when Playr starts up
            if (Disposed != null)
                Disposed();
        }
    }
}
