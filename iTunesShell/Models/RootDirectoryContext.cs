using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLibrary;
using iTunesShell.Interfaces;

namespace iTunesShell.Models
{
    class RootDirectoryContext : IDirectoryContext
    {
        private Dictionary<string, IMetaData> Children = new Dictionary<string, IMetaData>();

        /// <summary>
        /// We're the root of the namespace so we have no parent.
        /// </summary>
        public IDirectoryContext Parent => null;

        public string Name => "music";

        public RootDirectoryContext()
        {
            // TODO -- Case-sensitivity?
            Children["Artists"] = new ArtistRootContainer(this);
            Children["Albums"] = new AlbumRootContainer(this);
            Children["Playlists"] = new PlaylistRootContainer(this);
            Children["Ratings"] = new RatingRootContainer(this);
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            return Children.Values.ToList();
        }

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            if (Children.ContainsKey(Directory))
            {
                return Children[Directory] as IDirectoryContext;
            }
            else
            {
                return null;
            }
        }
    }
}
