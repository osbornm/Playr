using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playr.Api.Models;

namespace Playr.Api.Models
{
    public class Queue
    {
        public List<Song> PreviouslyPlayed { get; set; }
        public List<Song> UpNext { get; set; }
        public Song CurrentTrack { get; set; }

        public Queue()
        {
            PreviouslyPlayed = new List<Song>();
            UpNext = new List<Song>();
        }
    }


}
