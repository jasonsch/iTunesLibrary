using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class AlbumContainer : IDirectoryContext
    {
        private readonly Album album;

        public AlbumContainer(IDirectoryContext Parent, Album album)
        {
            this.Parent = Parent;
            this.album = album;
        }

        public IDirectoryContext Parent
        {
            get;
            private set;
        }


        public string Name => album.Name;

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            // An album is the end of its hierarchy.
            return null;
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            return album.Tracks.Skip(EnumerationContext).Take(Count).Select(s => new SongMetaData(s)).ToList<IMetaData>();
        }
    }
}
