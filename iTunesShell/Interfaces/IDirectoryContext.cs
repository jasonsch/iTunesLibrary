using System;
using System.Collections.Generic;
using System.Text;

namespace iTunesShell.Interfaces
{
    public interface IDirectoryContext
    {
        IDirectoryContext Parent { get; }
        string Name { get; }
        List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext); // TODO -- SearchContext should be some kind of object? EnumerationContext too?
        IDirectoryContext ChangeDirectory(string Directory);

        /// <summary>
        /// Some directories don't conceptually support "copying" files into them (e.g., if a song is rated four stars it
        /// can't be "copied" into root\ratings\5 (it would have to be moved into it).
        /// </summary>
        /* TODO
        bool CanCopyInto { get; }
        bool CanMoveInto { get; }
        */
    }
}
