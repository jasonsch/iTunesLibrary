using System;
using System.Collections.Generic;
using iTunesLib;

namespace iTunesLibrary
{
    public class Artist
    {
        /// <summary>
        /// The artist's name.
        /// </summary>
        public string Name { get; private set; }

        internal Artist(string name)
        {
            this.Name = name;
        }

        public List<Song> Songs
        {
            get
            {
                List<Song> SongList = new List<Song>();

                foreach (Album album in Albums)
                {
                    foreach (Song song in album)
                    {
                        SongList.Add(song);
                    }
                }

                return SongList;
            }
        }
        public List<Album> Albums { get; } = new List<Album>();

        public override string ToString()
        {
            return $"{Name} - {Albums.Count} Album(s)";
        }

        internal void AddAlbum(Album album)
        {
            if (!Albums.Contains(album))
            {
                Albums.Add(album);
            }
        }
    }
}
