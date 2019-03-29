using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics; // TODO
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
            ExternalCallback(Library.SongFromTrack((IITTrack)Track));
        }
    }

    public static class Library // TODO -- Rename?
    {
        private static readonly iTunesApp app = new iTunesApp();
        private static readonly List<SongPlayingCallbackWrapper> WrapperList = new List<SongPlayingCallbackWrapper>();

        public delegate void SongChangedCallback(Song s);

        private static readonly Dictionary<Artist, List<Album>> AlbumCache = new Dictionary<Artist, List<Album>>();
        private static List<Album> GlobalAlbumList = new List<Album>();
        private static readonly List<Song>[] RatingCache = new List<Song>[6];

        private static readonly List<Song> GlobalTrackList = new List<Song>();
        /// <summary>
        /// Keyed by file path.
        /// </summary>
        private static readonly Dictionary<string, Song> SongCache = new Dictionary<string, Song>();

        // TODO
        static int TotalTracks = 0;
        static int FileBasedTracks = 0;
        static int AlbumCount = 0;
        static int ArtistCount = 0;
        // TODO

        public static Song SongFromTrack(IITTrack Track)
        {
            Debug.Assert(Track.Kind == ITTrackKind.ITTrackKindFile);
            IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;

            return SongCache[FileTrack.Location];
        }

        static Library()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            for (int i = 0; i < RatingCache.Length; ++i)
            {
                RatingCache[i] = new List<Song>();
            }

            IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");

            foreach (IITTrack Track in Playlist.Tracks)
            {
                TotalTracks++;
                if (Track.Kind != ITTrackKind.ITTrackKindFile)
                {
                    continue;
                }
                FileBasedTracks++;

                try
                {
                    IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                    Console.WriteLine($"Processing track {FileTrack.Name} ({FileTrack.Location})");
                    Artist artist = ArtistFromArtistName(FileTrack, false); // TODO -- Do we ever care about the artist vs. the album artist?
                    Artist AlbumArtist = ArtistFromArtistName(FileTrack, true);
                    Album album = AlbumFromAlbumName(AlbumArtist, FileTrack.Album);
                    Song s = new Song(FileTrack, artist, AlbumArtist, album);
                    RatingCache[s.StarRating].Add(s);
                    album.AddSong(s);

                    GlobalTrackList.Add(s);
                    SongCache[FileTrack.Location] = s;
                } catch (Exception e)
                {
                    Console.WriteLine("exception ==> " + e.ToString());
                }
            }

            watch.Stop();

            // TODO
            Console.WriteLine($"Processed {FileBasedTracks} out of a total of {TotalTracks} tracks, requiring {watch.Elapsed.TotalSeconds} seconds");
            Console.WriteLine($"Found {AlbumCount} albums across {ArtistCount} artists.");
        }

        private static Album AlbumFromAlbumName(Artist AlbumArtist, string AlbumName)
        {
            List<Album> Albums;

            if (AlbumCache.ContainsKey(AlbumArtist))
            {
                Albums = AlbumCache[AlbumArtist];
            }
            else
            {
                AlbumCache[AlbumArtist] = Albums = new List<Album>();
            }

            Album album = Albums.Find(a => a.Name == AlbumName);
            if (album == null)
            {
                album = new Album(AlbumArtist, AlbumName);
                Albums.Add(album);
                GlobalAlbumList.Add(album);
                AlbumCount++; // TODO
            }

            return album;
        }

        private static Artist ArtistFromArtistName(IITFileOrCDTrack Track, bool AlbumArtist)
        {
            string ArtistName;

            if (AlbumArtist)
            {
                if (string.IsNullOrEmpty(Track.AlbumArtist))
                {
                    Console.WriteLine($"Looking for album artist for {Track.Name} but it's null!");
                    ArtistName = Track.Artist;
                }
                else
                {
                    ArtistName = Track.AlbumArtist;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Track.Artist))
                {
                    Console.WriteLine($"Looking for artist for {Track.Name} but it's null!");
                    ArtistName = Track.AlbumArtist;
                }
                else
                {
                    ArtistName = Track.Artist;
                }
            }

            if (string.IsNullOrEmpty(ArtistName))
            {
                Console.WriteLine($"Couldn't find artist name for {Track.Name} / {Track.Location}"); // TODO
                return null;
            }

            if (ArtistList.ContainsKey(ArtistName))
            {
                return ArtistList[ArtistName];
            }

            // TODO
            Console.WriteLine($"Adding new artist {ArtistName}");
            ArtistCount++;
            // TODO

            Artist artist = new Artist(ArtistName);
            ArtistList[ArtistName] = artist;
            return artist;
        }

        private static Dictionary<string, Artist> ArtistList = new Dictionary<string, Artist>();

        public static bool AddSongFile(string FileName)
        {
            return app.LibraryPlaylist.AddFile(FileName) != null;
        }

        /* TODO
        public static List<Album> GetAlbumsFromArtist(Artist artist)
        {
            // TODO: Thread-safety
            if (AlbumCache.ContainsKey(artist.Name))
            {
                return AlbumCache[artist.Name];
            }

            Dictionary<string, Album> AlbumList = new Dictionary<string, Album>();
            // IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");

            foreach (Song Track in GlobalTrackList)
            {
                if (Track.Artist.Name == artist.Name)
                {
                    if (!AlbumList.ContainsKey(Track.Album.Name))
                    {
                        AlbumList[Track.Album.Name] = new Album(artist, Track.Album.Name);
                    }

                    AlbumList[Track.Album.Name].AddSong(Track);
                }
            }

            AlbumCache[artist.Name] = AlbumList.Values.ToList();
            return AlbumCache[artist.Name];
        }

        public static List<Song> GetSongsFromAlbum(string AlbumName)
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

            return SongList;
        }
        */

        // TODO -- Cache this data? Also, is there a faster way to get the artists list?
        public static Artist[] Artists
        {
            get
            {
                /*
                IITPlaylist Playlist = app.LibrarySource.Playlists.get_ItemByName("Library");
                Dictionary<string, bool> ArtistHash = new Dictionary<string, bool>();

                foreach (IITTrack Track in Playlist.Tracks)
                {
                    if (Track.Kind != ITTrackKind.ITTrackKindFile)
                    {
                        continue;
                    }

                    IITFileOrCDTrack FileTrack = (IITFileOrCDTrack)Track;
                    //
                    // Not all tracks have an artist.
                    //
                    if (Track.Artist != null)
                    {
                        ArtistHash[Track.Artist] = true;
                    }
                }

                return ArtistHash.Keys.Select((name) => new Artist(name)).ToArray();
                */
                return ArtistList.Values.OrderBy(a => a.Name).ToArray();
            }
        }

        public static Album[] Albums
        {
            get
            {
                /*
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
                */
                return GlobalAlbumList.OrderBy(a => a.Name).ToArray(); // TODO -- Do once at beginning
            }
        }

        public static Song[] Songs
        {
            get
            {
                /*
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
                */
                return GlobalTrackList.ToArray(); // TODO  -- ToArray()
            }
        }

        /*
        TODO
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
        */

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

                return Library.SongFromTrack(Track);
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

        internal static List<Playlist> GetPlaylistsFromArtist(Artist artist)
        {
            foreach (Playlist playlist in Playlists)
            {
            }

            return null; // TODO
        }

        public static List<Song> GetSongsFromRating(int Rating)
        {
            if (Rating > 5 || Rating < 0)
            {
                throw new ArgumentException($"Rating ({Rating}) should be between 0 and 5 (inclusive)");
            }

            return RatingCache[Rating];
        }
    }
}
