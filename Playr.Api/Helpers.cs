﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Routing;
using iTunesLib;
using Playr.Api.Models;
using Raven.Client;
using Raven.Client.Embedded;
using SpeechLib;

namespace Playr.Api
{
    public static class Helpers
    {
        public static string _artworkUrlFormatString = ApplicationSettings.apiBaseUrl + "/songs/{0}/artwork";
        
        public static string GetToken(this HttpRequestMessage request)
        {
            
            var header = request.Headers.SingleOrDefault(x => x.Key == "x-playr-token");
            return header.Value.First();
        }

        public static IITTrack GetTrackById(this iTunesAppClass itunes, int id)
        {
            foreach (IITTrack t in itunes.LibraryPlaylist.Tracks)
            {
                if (t.TrackDatabaseID == id)
                {
                    return t;
                }
            }
            return null;
        }

        public static List<IITFileOrCDTrack> GetAlbumTracks(this iTunesAppClass itunes, string album)
        {
            var tracks = new List<IITFileOrCDTrack>();
            foreach (IITTrack t in itunes.LibraryPlaylist.Tracks)
            {
                if (t.Album != null && t.Album.Equals(album, StringComparison.InvariantCultureIgnoreCase))
                {
                    tracks.Add((IITFileOrCDTrack)t);
                }
            }
            return tracks;
        }

        public static Song toSong(this IITTrack t)
        {
            return new Song { Id = t.TrackDatabaseID, Album = t.Album, Artist = t.Artist, Rating = t.Rating, Title = t.Name, ArtworkUrl = String.Format(_artworkUrlFormatString, t.TrackDatabaseID) };
        }

        private static IDocumentStore docStore;

        public static IDocumentStore DocumentStore
        {
            get
            {
                if (docStore == null)
                    throw new InvalidOperationException("IDocumentStore has not been initialized.");
                return docStore;
            }
        }

        public static void InitializeDocumentStore()
        {
            docStore = new EmbeddableDocumentStore();
            docStore.Initialize();
        }

        public static void Speak(this iTunesApp itunes, string message)
        {
            var pasueMusic = itunes.PlayerState == ITPlayerState.ITPlayerStatePlaying;
            if (pasueMusic)
            {
                itunes.Pause();
            }
            var voice = new SpVoice();
            voice.Speak(message, SpeechVoiceSpeakFlags.SVSFDefault);
            if (pasueMusic)
            {
                itunes.Play();
            }
        }

       
    }


}
