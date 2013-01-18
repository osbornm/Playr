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

        public void Resume()
        {
            GuardDisposed();
            player.Play();
        }

        public void Play(string filePath)
        {
            GuardDisposed();
            EnsureWaveCleanUp();

            // DJ spin that shit
            player = new WaveOutEvent();
            fileWaveStream = new MediaFoundationReader(filePath);
            player.Init(fileWaveStream);
            player.PlaybackStopped += OnPlaybackStopped;

            player.Play();
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs evn)
        {
            if (!disposed && PlaybackStopped != null)
                PlaybackStopped();
        }

        private void EnsureWaveCleanUp()
        {
            if (player != null)
            {
                player.PlaybackStopped -= OnPlaybackStopped;
                if (player.PlaybackState != PlaybackState.Stopped)
                {
                    player.Stop();
                }
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
