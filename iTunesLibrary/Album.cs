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
        public string Name { get; internal set; }
        public Artist Artist { get; internal set; }

        internal Album(Artist Artist, string Name)
        {
            this.Artist = Artist;
            this.Name = Name;
        }

        public IEnumerator<Song> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
