using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playr.Web.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }
        public int Rating { get; set; }
        public string ArtworkUrl { get; set; }
    }
}
