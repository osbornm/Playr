using System;
using NAudio.Wave;
using Playr.Models;

namespace Playr.Services
{
    public class AudioService : IDisposable
    {
        private IWavePlayer player;
        private WaveStream fileWaveStream;
        private bool disposed;

        public TrackState AudioState { get; private set; }

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
            AudioState = TrackState.Paused;
        }

        public void Resume()
        {
            GuardDisposed();
            player.Play();
            AudioState = TrackState.Playing;
        }

        public void Play(string filePath)
        {
            GuardDisposed();
            EnsureWaveCleanUp();

            try
            {
                // DJ spin that shit
                player = new WaveOutEvent();
                fileWaveStream = new MediaFoundationReader(filePath);
                player.Init(fileWaveStream);
                player.PlaybackStopped += OnPlaybackStopped;

                player.Play();
                AudioState = TrackState.Playing;
            }
            catch
            {
                // If playback failed because of an incompatible file, log it and move on.
                Log.Warning("Could not play file: {0}", filePath);
                OnPlaybackStopped(null, null);
                AudioState = TrackState.Stopped;
            }
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
                fileWaveStream = null;
            }
            if (player != null)
            {
                try
                {
                    player.Dispose();
                }
                catch { } // Sometimes it throws NullRef if we dispose but never started playing anything

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
