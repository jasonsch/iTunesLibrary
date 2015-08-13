using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLib;

namespace iTunesLibrary
{
    public enum RatingPredicate
    {
        EqualTo,
        LessThan,
        LessThanOrEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo
    }

    public class Song
    {
        private readonly IITTrack Track;

        public Song(IITTrack Track)
        {
            this.Track = Track;
        }

        //
        // Needed for serialization.
        //
        public Song()
        {
        }

        public override string ToString()
        {
            return String.Format("Song: Name ==> {0}, Album ==> {1}, Length ==> {2}, Location ==> {3}", Name, Album, Length, Location);
        }

        public string Artist
        {
            get
            {
                return Track.Artist;
            }
        }

        public string Album
        {
            get
            {
                return Track.Album;
            }
        }

        public string Name
        {
            get
            {
                return Track.Name;
            }
        }

        public uint LengthInSeconds
        {
            get
            {
                return (uint)Track.Duration;
            }
        }

        public string Length
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                uint SongLength = LengthInSeconds;
                while (SongLength != 0)
                {
                    sb.Insert(0, SongLength % 60);
                    SongLength /= 60;
                    if (SongLength != 0)
                    {
                        sb.Insert(0, ":");
                    }
                }

                return sb.ToString();
            }
        }

        public string Location
        {
            get
            {
                IITFileOrCDTrack FileTrack = Track as IITFileOrCDTrack;
                if (FileTrack == null)
                {
                    return "";
                }
                else
                {
                    return FileTrack.Location;
                }
            }
        }
    }

    public class Playlist
    {
        private readonly IITPlaylist _Playlist;
        public List<Song> SongList = null;

        public Playlist(IITPlaylist Playlist)
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Playlist: {0}\n", _Playlist.Name);
            Song[] SongList = Songs;

            uint i = 1;
            foreach (Song s in SongList)
            {
                sb.AppendFormat("({0}) {1} - {2}\n", i, s.Album, s.Name);
                i += 1;
            }

            return sb.ToString();
        }

        public Song[] Songs
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

                            SongList.Add(new Song(Track));
                        }
                    }
                }

                return SongList.ToArray();
            }
        }
    }

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

    public class Library
    {
        private readonly iTunesApp app;
        private List<SongPlayingCallbackWrapper> WrapperList = new List<SongPlayingCallbackWrapper>();

        public delegate void SongChangedCallback(Song s);

        public Library()
        {
            app = new iTunesApp();
        }

        public Song[] GetSongsFromAlbum(string AlbumName)
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

        public Song[] GetSongsByArtist(string ArtistName)
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

        public Playlist GetPlaylist(string PlaylistName)
        {
            return new Playlist(app.LibrarySource.Playlists.get_ItemByName(PlaylistName));
        }

        public Song[] GetSongsByRating(int RatingValue, RatingPredicate Predicate)
        {
            IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");
            List<Song> SongList = new List<Song>();

            foreach (IITTrack Track in Playlist.Tracks)
            {
                if (Track.Kind == ITTrackKind.ITTrackKindFile)
                {
                    IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                    int Rating = FileTrack.Rating / 20; // TODO -- This is using the "stars" metric.
                    bool ShouldCopy = false;

                    switch (Predicate)
                    {
                        case RatingPredicate.EqualTo:
                            ShouldCopy = (Rating == RatingValue);
                            break;
                        case RatingPredicate.GreaterThan:
                            ShouldCopy = (Rating > RatingValue);
                            break;
                        case RatingPredicate.GreaterThanOrEqualTo:
                            ShouldCopy = (Rating >= RatingValue);
                            break;
                        case RatingPredicate.LessThan:
                            ShouldCopy = (Rating < RatingValue);
                            break;
                        case RatingPredicate.LessThanOrEqualTo:
                            ShouldCopy = (Rating <= RatingValue);
                            break;
                    }

                    if (ShouldCopy)
                    {
                        SongList.Add(new Song(Track));
                    }
                }
            }

            return SongList.ToArray();
        }

        public bool Muted
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

        public Song CurrentSong
        {
            get
            {
                IITTrack Track = app.CurrentTrack;

                if (Track == null)
                {
                    return null;
                }

                app.OnPlayerPlayEvent += app_OnPlayerPlayEvent;
                return new Song(Track);
            }
        }

        private void app_OnPlayerPlayEvent(object iTrack)
        {
            throw new NotImplementedException();
        }

        public event SongChangedCallback TrackChanged
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
