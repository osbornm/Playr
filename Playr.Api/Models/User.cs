using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Playr.Api.Models
{
    public class User
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public List<Song> Favorites { get; set; }

        public User()
        {
            Favorites = new List<Song>();
        }
    }
}
