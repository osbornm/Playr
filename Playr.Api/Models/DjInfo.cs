using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Playr.Api.Models;

namespace Playr.Api.Models
{
    public class DjInfo
    {
        public List<Song> History { get; set; }
        public List<Song> Queue { get; set; }
        public Song CurrentTrack { get; set; }

        public DjInfo()
        {
            History = new List<Song>();
            Queue = new List<Song>();
        }
    }


}
