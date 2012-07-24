using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using iTunesLib;
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

        [RequireToken]
        public void Pause()
        {
            itunes.Pause();
        }

        [RequireToken]
        public void Play()
        {
            itunes.Play();
        }

        [RequireToken]
        public void Skip()
        {
            itunes.NextTrack();
        }

        //TODO: Volume up and down?
    }
}
