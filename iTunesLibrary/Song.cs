using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLib;

namespace iTunesLibrary
{
    public class Song
    {
        private readonly IITTrack Track;

        internal Song(IITTrack Track)
        {
            this.Track = Track;
        }

        public override string ToString()
        {
            return String.Format("{0} - {1} by {2}", Album, Name, Artist);
        }

        public Artist Artist
        {
            get
            {
                return new Artist(Track.Artist);
            }
        }

        public Album Album
        {
            get
            {
                return new Album(Artist, Track.Album);
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
                    return String.Empty;
                }
                else
                {
                    return FileTrack.Location;
                }
            }
        }

        public int StarRating
        {
            get
            {
                IITFileOrCDTrack FileTrack = Track as IITFileOrCDTrack;
                if (FileTrack == null)
                {
                    return 0;
                }
                else
                {
                    //
                    // Map the rating -- which is on a scale of 0 - 100 -- to a star-based rating (i.e., 0 to 5 stars).
                    //
                    return FileTrack.Rating / 20;
                }
            }
        }

    }
}
