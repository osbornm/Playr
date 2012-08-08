using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using iTunesLib;
using Newtonsoft.Json.Linq;
using Playr.Api.Models;

namespace Playr.Api.Controller
{
    public class ControlController : ApiController
    {
        iTunesAppClass itunes;

        public ControlController()
        {
            itunes = new iTunesAppClass();
        }

        [RequireToken, HttpPut]
        public void Pause()
        {
            itunes.Pause();
        }

        [RequireToken, HttpPut]
        public void Play()
        {
            itunes.Play();
        }

        [RequireToken, HttpPut]
        public object PlayPause()
        {
            var pasueMusic = itunes.PlayerState == ITPlayerState.ITPlayerStatePlaying;
            if (pasueMusic)
            {
                itunes.Pause();
                return new { Status = "Paused" };
            }
            else
            {
                itunes.Play();
                return new { Status = "Playing" };
            }

        }

        [RequireToken, HttpPost]
        public void Next()
        {
            itunes.NextTrack();
        }

        [RequireToken, HttpPost]
        public void Previous()
        {
            itunes.PreviousTrack();
        }

        [RequireToken, HttpPost]
        public void VolumeUp()
        {
            itunes.SoundVolume += 5;
        }

        [RequireToken, HttpPost]
        public void VolumeDown()
        {
            itunes.SoundVolume -= 5;
        }

        [RequireToken, HttpPost]
        public void Speak(JObject json)
        {
            itunes.Speak(json["message"].ToString());      
        }
    }
}
