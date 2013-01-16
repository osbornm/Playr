using System;
using NAudio.Wave;

namespace Playr.Services
{
    public class AudioService : IDisposable
    {
        private IWavePlayer player;
        private WaveStream fileWaveStream;
        private bool disposed; 

        public event Action PlaybackStopped;

        public TimeSpan CurrentTime
        {
            get
            {
                if (fileWaveStream != null)
                {
                    return fileWaveStream.CurrentTime;
                }
                return TimeSpan.Zero;
            }
        }

        public void Dispose()
        {
            disposed = true;
            EnsureWaveCleanUp();
        }

        public void Pause()
        {
            GuardDisposed();

            player.Pause();
        }

        public void Play(string filePath)
        {
            GuardDisposed();

            EnsureWaveCleanUp();

            // DJ spin that shit
            player = new WaveOutEvent();
            fileWaveStream = new MediaFoundationReader(filePath);
            player.Init(fileWaveStream);
            player.PlaybackStopped += (sender, evn) =>
            {
                if (!disposed && PlaybackStopped != null)
                    PlaybackStopped();
            };

            player.Play();
        }

        private void EnsureWaveCleanUp()
        {
            if (player != null && player.PlaybackState != PlaybackState.Stopped)
            {
                player.Stop();
            }
            if (fileWaveStream != null)
            {
                fileWaveStream.Dispose();
            }
            if (player != null)
            {
                player.Dispose();
                player = null;
            }
        }

        private void GuardDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("AudioService");
        }
    }
}
