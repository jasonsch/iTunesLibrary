using System.Collections.Generic;
using System.Linq;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class AlbumRootContainer : IDirectoryContext, IMetaData
    {
        public AlbumRootContainer(IDirectoryContext Parent)
        {
            this.Parent = Parent;
        }

        public IDirectoryContext Parent { get; }

        public string Name => ShortDescription;
        public string ShortDescription => "[Albums]";
        public string LongDescription => "[Albums]"; // TODO

        public string FileName => throw new System.NotImplementedException();

        private List<Album> AlbumList = null; // TODO

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            if (AlbumList == null) // TODO
            {
                AlbumList = Library.Albums.ToList(); // TODO List vs array
            }

            Album album = AlbumList.Where(a => a.Name == Directory).FirstOrDefault();
            if (album == null)
            {
                return null;
            }

            return new AlbumContainer(this, album);
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            if (AlbumList == null) // TODO
            {
                AlbumList = Library.Albums.ToList(); // TODO List vs array
            }

            return AlbumList.Skip(EnumerationContext).Take(Count).Select(a => new AlbumMetaData(a)).ToList<IMetaData>();
        }
    }
}
