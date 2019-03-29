using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class ArtistMetaData : IMetaData
    {
        private readonly Artist artist;

        public ArtistMetaData(Artist artist)
        {
            this.artist = artist;
        }

        public string ShortDescription => artist.Name;
        public string LongDescription => $"{artist.Name} (Album Count = ${artist.Albums.Count})";

        public string FileName => throw new NotImplementedException();
    }
}
