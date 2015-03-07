using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTunesLibrary;

namespace SharePlaylist
{
    class Program
    {
        static void Main(string[] args)
        {
            // Print out all the playlists
            if (args.Length == 0)
            {
                foreach (Playlist p in Library.Playlists)
                {
                    System.Console.WriteLine("Playlist ==> " + p.Name);
                }

                return;
            }

            System.Console.WriteLine("Looking for a playlist called: " + args[0]);

            Playlist playlist = Library.Playlists.SingleOrDefault(p => p.Name == args[0]);
            if (playlist == null)
            {
                System.Console.WriteLine("Couldn't find playlist: " + args[0]);
            }
            else
            {
                foreach (Song s in playlist)
                {
                    System.Console.WriteLine("Song ==> " + s);
                }
            }
        }
    }
}
