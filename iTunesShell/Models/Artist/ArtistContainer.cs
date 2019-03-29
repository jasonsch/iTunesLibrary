using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class ArtistContainer : IDirectoryContext
    {
        private readonly Artist artist;

        public ArtistContainer(IDirectoryContext Parent, Artist artist)
        {
            this.artist = artist;
            this.Parent = Parent;
        }

        public IDirectoryContext Parent
        {
            get;
            private set;
        }

        public string Name => artist.Name;

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            return new AlbumContainer(this, artist.Albums.Where(a => a.Name == Directory).First());
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            return artist.Albums.Select(a => new AlbumMetaData(a)).ToList<IMetaData>();
        }
    }
}
