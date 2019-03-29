using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;
using iTunesLibrary;

namespace iTunesShell.Models
{
    class RatingContainer : IDirectoryContext, IMetaData
    {
        /// <summary>
        /// A file can be rated 1-5. If the value is 0 then the file is unrated.
        /// </summary>
        private readonly int Rating;
        private List<Song> Songs;

        public RatingContainer(IDirectoryContext Parent, int Rating)
        {
            this.Parent = Parent;
            this.Rating = Rating;

            Songs = Library.GetSongsFromRating(Rating);
        }

        public IDirectoryContext Parent { get; }

        public string Name => GenerateName();

        private string GenerateName()
        {
            if (Rating == 0)
            {
                return "(unrated)";
            }
            else
            {
                return new string('*', Rating);
            }
        }

        public string ShortDescription => Name;
        public string LongDescription => Name;

        public string FileName => throw new NotImplementedException();

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            // End of the line.
            return null;
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            return Songs.Select(s => new SongMetaData(s)).ToList<IMetaData>();
        }
    }
}
