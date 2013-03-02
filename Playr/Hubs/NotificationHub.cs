using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Playr.Api.Music.Models;

namespace Playr.Hubs
{
    public class NotificationHub: Hub
    {
        public void CurrentTrackChanged(CurrentTrack track)
        {
            Clients.All.CurrentTrackChanged(track);
        }
    }
}
