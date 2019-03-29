using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesShell.Interfaces;

namespace iTunesShell.Models
{
    class RatingRootContainer : IMetaData, IDirectoryContext
    {
        private readonly Dictionary<string, IMetaData> Ratings = new Dictionary<string, IMetaData>();

        public RatingRootContainer(IDirectoryContext Parent)
        {
            this.Parent = Parent;

            Ratings["(unrated)"] = new RatingContainer(this, 0);

            for (int i = 1; i <= 5; ++i)
            {
                string Name = new string('*', i);

                Ratings[Name] = new RatingContainer(this, i);
            }
        }

        public string ShortDescription => Name;

        public string LongDescription => ShortDescription;

        public IDirectoryContext Parent { get; }

        public string Name => "[Ratings]";

        public string FileName => throw new NotImplementedException();

        public IDirectoryContext ChangeDirectory(string Directory)
        {
            int Rating;

            if (Directory == "(unrated)")
            {
                Rating = 0;
            }
            else
            {
                Rating = Directory.Length;
            }

            return new RatingContainer(this, Rating);
        }

        public List<IMetaData> GetItems(ref int EnumerationContext, int Count, string SearchContext)
        {
            List<IMetaData> Items = new List<IMetaData>();

            foreach (var Value in Ratings.Values)
            {
                Items.Add(Value as IMetaData);
            }

            return Items;
        }
    }
}
