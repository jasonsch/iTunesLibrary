using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLibrary;
using Mono.Options;

namespace PlaylistCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            OptionSet options = new OptionSet();
            Int32 DayCount = Int32.MaxValue;

            options.Add("numberofdays=", value => DayCount = Int32.Parse(value));


            List<string> RemainingArgs = options.Parse(args);
            //
            // Validate args
            //
            if (RemainingArgs.Count != 2)
            {
                PrintUsage("Wrong number of arguments!");
            }

            if (!Directory.Exists(RemainingArgs[1]))
            {
                Directory.CreateDirectory(RemainingArgs[1]);
            }

            CopyPlaylist(RemainingArgs[0], RemainingArgs[1], DayCount);
        }

        private static void CopyPlaylist(string PlaylistName, string Destination, int DayCount)
        {
            Playlist Playlist = Library.Playlists.SingleOrDefault(p => p.Name == PlaylistName);

            if (Playlist == null)
            {
                PrintUsage(String.Format("Couldn't find playlist {0}", PlaylistName));
            }

            foreach (Song Song in Playlist.Songs)
            {
                if ((DateTime.Now - Song.DateAdded).TotalDays < DayCount)
                {
                    CopySong(Song, Destination);
                }
            }
        }

        private static void CopySong(Song Song, string Destination)
        {
            string DestinationPath;
            string OriginalFilePath = Song.Location;
            string Artist, Album, SongName;
            string FullDirectory;

            SongName = Path.GetFileName(OriginalFilePath);
            FullDirectory = Path.GetDirectoryName(OriginalFilePath);
            Album = FullDirectory.Substring(FullDirectory.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            FullDirectory = FullDirectory.Substring(0, FullDirectory.LastIndexOf(Path.DirectorySeparatorChar));
            Artist = FullDirectory.Substring(FullDirectory.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            DestinationPath = Path.Combine(Destination, Artist);
            if (!Directory.Exists(DestinationPath))
            {
                Directory.CreateDirectory(DestinationPath);
            }

            DestinationPath = Path.Combine(DestinationPath, Album);
            if (!Directory.Exists(DestinationPath))
            {
                Directory.CreateDirectory(DestinationPath);
            }

            DestinationPath = Path.Combine(DestinationPath, SongName);
            File.Copy(Song.Location, DestinationPath);
        }

        private static void PrintUsage(string ErrorString)
        {
            System.Console.WriteLine("Error: {0}", ErrorString);
            System.Console.WriteLine("Usage: PlaylistCopy [-numberofdays=<age of songs in days to copy>] <Playlist name> <Destination directory>");
            System.Environment.Exit(0);
        }
    }
}
