using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class PlaylistContainer : IDirectoryContext
    {
        private readonly Playlist playlist;
        private readonly List<IMetaData> Songs;

        public PlaylistContainer(IDirectoryContext Parent, Playlist playlist)
        {
            this.Parent = Parent;
            this.playlist = playlist;

            Songs = playlist.Songs.Select(s => new SongMetaData(s)).ToList<IMetaData>();
        }

        public IDirectoryContext Parent { get; }

        public string Name => playlist.Name;

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            // This is the end of the line.
            return null;
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            return Songs.Skip(EnumerationContext).Take(Count).ToList();
        }
    }
}
