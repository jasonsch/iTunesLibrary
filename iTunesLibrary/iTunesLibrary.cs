using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLib;

namespace iTunesLibrary
{
    // TODO -- Describe why we have this class.
    internal class SongPlayingCallbackWrapper
    {
        public Library.SongChangedCallback ExternalCallback;

        public SongPlayingCallbackWrapper(Library.SongChangedCallback ExternalCallback)
        {
            this.ExternalCallback = ExternalCallback;
        }

        public void OnPlayerPlayEventHandler(object Track)
        {
            ExternalCallback(new Song((IITTrack)Track));
        }
    }

    public static class Library // TODO -- Rename?
    {
        private static readonly iTunesApp app = new iTunesApp();
        private static readonly List<SongPlayingCallbackWrapper> WrapperList = new List<SongPlayingCallbackWrapper>();

        public delegate void SongChangedCallback(Song s);

        public static Song[] GetSongsFromAlbum(string AlbumName) // TODO
        {
            List<Song> SongList = new List<Song>();
            IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");

            foreach (IITTrack Track in Playlist.Tracks)
            {
                if (Track.Kind != ITTrackKind.ITTrackKindFile)
                {
                    continue;
                }

                IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                if (Track.Album == AlbumName)
                {
                    SongList.Add(new Song(Track));
                }
            }

            return SongList.ToArray();
        }

        public static Artist[] Artists
        {
            get
            {
                IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");
                Dictionary<string, bool> ArtistHash = new Dictionary<string, bool>();

                foreach (IITTrack Track in Playlist.Tracks)
                {
                    if (Track.Kind != ITTrackKind.ITTrackKindFile)
                    {
                        continue;
                    }

                    IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                    ArtistHash[Track.Artist] = true;
                }

                return ArtistHash.Keys.Select((name) => new Artist(name)).ToArray();
            }
        }

        public static Album[] Albums
        {
            get
            {
                IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");
                Dictionary<Tuple<string, string>, bool> AlbumHash = new Dictionary<Tuple<string, string>, bool>();

                foreach (IITTrack Track in Playlist.Tracks)
                {
                    if (Track.Kind != ITTrackKind.ITTrackKindFile)
                    {
                        continue;
                    }

                    IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                    AlbumHash[new Tuple<string, string>(FileTrack.Artist, FileTrack.Album)] = true;
                }

                return AlbumHash.Keys.Select((tuple) => new Album(new Artist(tuple.Item1), tuple.Item2)).ToArray();
            }
        }

        public static Song[] Songs // IEnumerable?
        {
            get
            {
                IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");
                List<Song> SongList = new List<Song>();

                foreach (IITTrack Track in Playlist.Tracks)
                {
                    if (Track.Kind != ITTrackKind.ITTrackKindFile)
                    {
                        continue;
                    }

                    IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                    SongList.Add(new Song(Track));
                }

                return SongList.ToArray();
            }
        }

        public static Song[] GetSongsByArtist(string ArtistName) // TODO
        {
            IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");
            List<Song> SongList = new List<Song>();

            foreach (IITTrack Track in Playlist.Tracks)
            {
                if (Track.Kind != ITTrackKind.ITTrackKindFile)
                {
                    continue;
                }

                IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                if (Track.Artist == ArtistName)
                {
                    SongList.Add(new Song(Track));
                }
            }

            return SongList.ToArray();
        }

        public static IEnumerable<Playlist> Playlists
        {
            get
            {
                foreach (IITPlaylist Playlist in app.LibrarySource.Playlists)
                {
                    yield return new Playlist(Playlist);
                }
            }
        }

        public static bool Muted
        {
            get
            {
                return app.Mute;
            }

            set
            {
                app.Mute = value;
            }
        }

        public static Song CurrentSong
        {
            get
            {
                IITTrack Track = app.CurrentTrack;

                if (Track == null)
                {
                    return null;
                }

                return new Song(Track);
            }
        }

        public static event SongChangedCallback TrackChanged
        {
            add
            {
                SongPlayingCallbackWrapper Wrapper = new SongPlayingCallbackWrapper(value);
                app.OnPlayerPlayEvent += Wrapper.OnPlayerPlayEventHandler;
            }

            remove
            {
                SongPlayingCallbackWrapper Wrapper = WrapperList.Find(wrapper => wrapper.ExternalCallback == value);
                app.OnPlayerPlayEvent -= Wrapper.OnPlayerPlayEventHandler;
            }
        }
    }
}
