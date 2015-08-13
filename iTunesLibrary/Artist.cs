using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesLibrary
{
    public class Artist
    {
        /// <summary>
        /// The artist's name.
        /// </summary>
        public string Name { get; internal set; }

        internal Artist(string name)
        {
            this.Name = name;
        }

        public Song[] Songs
        {
            get
            {
                return new Song[] { }; // TODO
            }
        }

        public Album[] Albums
        {
            get
            {
                return new Album[] { }; // TODO
            }
        }

        public Playlist[] Playlists
        {
            get
            {
                return new Playlist[] { }; // TODO
            }
        }
    }
}
