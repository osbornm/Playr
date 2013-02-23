using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playr.DataModels;
using Playr.Helpers;

namespace Playr.Services
{
    public class ControlService : IDisposable
    {
        private AudioService audio;
        private MusicLibraryService library = new MusicLibraryService();
        private Deque<DbTrack> playlist = new Deque<DbTrack>();
        private Deque<DbTrack> previous = new Deque<DbTrack>();
        private DbTrack currentTrack;
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

        public DbTrack CurrentTrack
        {
            get
            {
                return currentTrack;
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
            if (currentTrack != null)
                AddToPrevious(currentTrack);
            //if there is nothing in the playlist move on
            if (playlist.Count == 0)
            {
                return;
            }
            currentTrack = playlist.PopFirst();

            // Fill back up the queue
            var songsToAdd = queueLength - playlist.Count;
            for (int i = 0; i < songsToAdd; i++)
            {
                playlist.AddLast(library.GetRandomTrack());
            }

            // DJ spin that shit
            audio.Play(currentTrack.Location);

            OnQueueChanged();
            OnCurrentTrackChanged();
        }

        public void Previous()
        {
            if (previous.Count == 0)
                return;

            playlist.AddFirst(currentTrack);
            currentTrack = previous.PopLast();
            // DJ spin that shit
            audio.Play(currentTrack.Location);

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
                CurrentTrackChanged(currentTrack);
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
        }
    }
}
