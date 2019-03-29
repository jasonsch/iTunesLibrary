using iTunesLibrary;
using iTunesShell.Interfaces;

namespace iTunesShell.Models
{
    class SongMetaData : IMetaData
    {
        private readonly Song song;

        public SongMetaData(Song song)
        {
            this.song = song;
        }

        public string ShortDescription => $"{song.Artist.Name} - {song.Name}";
        public string LongDescription => $"{ShortDescription} ({song.Length})";

        public string FileName => song.Location;
    }
}
