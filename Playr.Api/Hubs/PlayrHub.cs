using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playr.Api.Models;
using SignalR.Hubs;

namespace Playr.Api.Hubs
{
    [HubName("playr")]
    public class PlayrHub: Hub
    {
    }
}
