using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Playr.Web.Models
{
    public class ApiUser
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public List<Song> Favorites { get; set; }

        public ApiUser()
        {
            Favorites = new List<Song>();
        }
    }

    public class UserToken
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}