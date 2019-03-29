using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class PlaylistMetaData : IMetaData
    {
        public readonly Playlist playlist;

        public PlaylistMetaData(Playlist playlist)
        {
            this.playlist = playlist;
        }

        public string ShortDescription => playlist.Name;
        public string LongDescription => $"{ShortDescription} ({playlist.Songs.Count})";

        public string FileName => throw new NotImplementedException();
    }
}
