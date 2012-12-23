using System;
using System.Security.Cryptography;
using System.Text;

namespace Playr.DataModels
{
    public class DbAlbum
    {
        public string Id { get; set; }

        public string ArtistName { get; set; }
        public string Genre { get; set; }
        public string Name { get; set; }
    }
}
