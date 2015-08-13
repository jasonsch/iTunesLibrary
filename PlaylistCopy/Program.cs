using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLibrary;

namespace PlaylistCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            // Validate args
            //
            if (args.Length != 2)
            {
                PrintUsage("Wrong number of arguments!");
            }

            if (!Directory.Exists(args[1]))
            {
                Directory.CreateDirectory(args[1]);
            }

            CopyPlaylist(args[0], args[1]);
        }

        private static void CopyPlaylist(string PlaylistName, string Destination)
        {
            Playlist Playlist = Library.Playlists.SingleOrDefault(p => p.Name == PlaylistName);

            if (Playlist == null)
            {
                PrintUsage(String.Format("Couldn't find playlist {0}", PlaylistName));
            }

            foreach (Song Song in Playlist.Songs)
            {
                CopySong(Song, Destination);
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
            System.Console.WriteLine("Usage: PlaylistCopy <Playlist name> <Destination directory>");
            System.Environment.Exit(0);
        }
    }
}
