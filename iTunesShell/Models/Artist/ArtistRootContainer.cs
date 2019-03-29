using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class ArtistRootContainer : IMetaData, IDirectoryContext
    {
        public ArtistRootContainer(IDirectoryContext Parent)
        {
            this.Parent = Parent;
        }

        public string ShortDescription => "[Artists]";
        public string LongDescription => "[Artists]"; // TODO

        public IDirectoryContext Parent { get; }

        public string Name => ShortDescription;

        public string FileName => throw new NotImplementedException();

        private Artist[] ArtistList = null;

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext /* TODO */)
        {
            if (ArtistList == null)
            {
                ArtistList = Library.Artists;
            }

            return ArtistList.Skip(EnumerationContext).Take(Count).Select(a => new ArtistMetaData(a)).ToList<IMetaData>();
        }

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            Artist artist;

            // TODO -- Make this a lazy property
            if (ArtistList == null)
            {
                ArtistList = Library.Artists;
            }

            artist = ArtistList.Where(a => a.Name == Directory).FirstOrDefault();
            if (artist == null)
            {
                return null;
            }

            return new ArtistContainer(this, artist);
        }
    }
}
