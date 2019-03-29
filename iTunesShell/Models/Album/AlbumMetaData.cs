using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class AlbumMetaData : IMetaData
    {
        private readonly Album album;

        public AlbumMetaData(Album album)
        {
            this.album = album;
        }

        // TODO
        // public string ShortDescription => album.Name;
        public string ShortDescription => LongDescription;
        public string LongDescription => $"{album.Name} ({album.TrackCount})";

        public string FileName => throw new NotImplementedException();
    }
}
