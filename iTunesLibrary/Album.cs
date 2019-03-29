using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLib;

namespace iTunesLibrary
{
    public class Album : IEnumerable<Song>
    {
        public string Name { get; private set; }
        public Artist Artist { get; private set; }
        public int TrackCount => Tracks.Count;
        public List<Song> Tracks { get; private set; }

        public string FileSystemPath
        {
            get
            {
                if (Tracks.Count == 0)
                {
                    return "c:\null_or_empty\foo.mp3"; // TODO
                }

                return System.IO.Path.GetDirectoryName(Tracks[0].Location);
            }
        }

        internal Album(Artist Artist, string Name)
        {
            this.Artist = Artist;
            this.Name = Name;
            // TODO
            // this.Tracks = Library.GetSongsFromAlbum(Name).ToList();
            this.Tracks = new List<Song>();
        }

        public IEnumerator<Song> GetEnumerator()
        {
            // TODO
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Name;
        }

        internal void AddSong(Song s)
        {
            Tracks.Add(s);
        }
    }
}
