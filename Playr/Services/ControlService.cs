using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playr.DataModels;

namespace Playr.Services
{
    public class ControlService : IDisposable
    {
        private AudioService audio;
        private MusicLibraryService library = new MusicLibraryService();
        private Queue<DbTrack> playlist = new Queue<DbTrack>();
        private DbTrack currentTrack;
        private int queueLength;

        public ControlService(AudioService audio)
        {
            this.audio = audio;
            audio.PlaybackStopped += PlayNext;

            queueLength = Convert.ToInt32(ConfigurationManager.AppSettings["Playr:QueueLength"]);
            for (int i = 0; i < queueLength; i++)
            {
                playlist.Enqueue(library.GetRandomTrack());
            }

            PlayNext();
        }

        private void PlayNext()
        {
            // Get the song
            currentTrack = playlist.Dequeue();
            var filePath = currentTrack.Location;

            // Fill back up the queue
            var songsToAdd = playlist.Count - queueLength;
            for (int i = 0; i < songsToAdd; i++)
            {
                playlist.Enqueue(library.GetRandomTrack());
            }

            // DJ spin that shit
            audio.Play(filePath);
        }

        // Queue 
        // QueueSong -> add to bottom of list
        // PlayNow -> stop current play this now
        // CurrentTrack
        // Pasue
        // Play
        // NextSong
        // PrevSong
        // Pausing, 
        // playing, 
        // track change, 
        // queue change 

        public void Dispose()
        {
        }
    }
}
