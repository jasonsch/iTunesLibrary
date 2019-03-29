using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class PlaylistRootContainer : IMetaData, IDirectoryContext
    {
        private readonly List<IMetaData> Playlists;
        public IDirectoryContext Parent { get; }

        public PlaylistRootContainer(IDirectoryContext Parent)
        {
            this.Parent = Parent;

            // We assume there won't be too many playlists and just read them all.
            Playlists = Library.Playlists.Select(p => new PlaylistMetaData(p)).ToList<IMetaData>();
        }

        public string ShortDescription => "[Playlists]";
        public string LongDescription => $"Playlists ({Playlists.Count})";

        public string Name => ShortDescription;

        public string FileName => throw new NotImplementedException();

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            // TODO -- Shouldn't be using ShortDescription like this.
            PlaylistMetaData playlist = Playlists.Where(p => p.ShortDescription == Directory).FirstOrDefault() as PlaylistMetaData;

            if (playlist == null)
            {
                return null;
            }

            return new PlaylistContainer(this, playlist.playlist);
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            return Playlists.Skip(EnumerationContext).Take(Count).ToList();
        }
    }
}
