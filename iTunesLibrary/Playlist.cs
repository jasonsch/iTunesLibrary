using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLib;

namespace iTunesLibrary
{
    public class Playlist : IEnumerable<Song>
    {
        private readonly IITPlaylist _Playlist;
        private List<Song> SongList = null;

        internal Playlist(IITPlaylist Playlist)
        {
            _Playlist = Playlist;
        }

        public string Name
        {
            get
            {
                return _Playlist.Name;
            }
        }

        public void AddSong(string FilePath)
        {
            IITLibraryPlaylist Playlist = _Playlist as IITLibraryPlaylist;

            Playlist.AddFile(FilePath);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Playlist: {0}\n", _Playlist.Name);

            uint i = 1;
            foreach (Song s in Songs)
            {
                sb.AppendFormat("({0}) {2}\n", i, s.ToString());
                i += 1;
            }

            return sb.ToString();
        }

        public List<Song> Songs
        {
            get
            {
                if (SongList == null)
                {
                    SongList = new List<Song>();

                    foreach (IITTrack Track in _Playlist.Tracks)
                    {
                        if (Track.Kind == ITTrackKind.ITTrackKindFile)
                        {
                            IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;

                            SongList.Add(Library.SongFromTrack(Track));
                        }
                    }
                }

                return SongList;
            }
        }

        public IEnumerator<Song> GetEnumerator()
        {
            foreach (Song s in Songs)
            {
                yield return s;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
