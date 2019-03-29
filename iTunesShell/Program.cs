using System;
using System.Collections.Generic;
using System.Text;
using iTunesShell.Interfaces;
using iTunesShell.Models;
using iTunesLibrary;

namespace iTunesShell
{
    class Program
    {
        private static IDirectoryContext Context;

        static void Main(string[] args)
        {
            Context = new RootDirectoryContext();

            if (args.Length != 0)
            {
                if (args.Length == 1)
                {
                    foreach (string Line in System.IO.File.ReadAllLines(args[0]))
                    {
                        ProcessCommand(Line);
                    }
                }
            }
            else
            {
                //
                // TODO -- Register control handler to eat CTRL+C
                //
                while (true)
                {
                    Console.Write(BuildPrompt());
                    // TODO -- Need to support tab completion
                    string Command = Console.ReadLine();
                    ProcessCommand(Command);
                }
            }
        }

        static int ListingOffset = 0; // TODO

        static void ProcessCommand(string Command)
        {
            if (Command.StartsWith("dir"))
            {
                foreach (var Item in Context.GetItems(ref ListingOffset, 50 /* TODO */, ""))
                {
                    Console.WriteLine(Item.ShortDescription);
                }
                ListingOffset += 50;
            }
            else if (Command.StartsWith("cd"))
            {
                ListingOffset = 0; // TODO
                string DirectoryName = Command.Substring(3);

                // TODO -- Handle ..\[...]\<directory name>
                if (DirectoryName == "..")
                {
                    //
                    // Silently "fail" if we're already at the root.
                    //
                    if (Context.Parent != null)
                    {
                        Context = Context.Parent;
                    }
                }
                else
                {
                    var NewContext = Context.ChangeDirectory(DirectoryName);
                    if (NewContext == null)
                    {
                        Console.WriteLine($"Invalid directory {DirectoryName}");
                    }
                    else
                    {
                        Context = NewContext;
                    }
                }
            }
            else if (Command.StartsWith("add"))
            {
                bool b;
                string FileName;

                FileName = Command.Split(new char[] { ' ' }, 2)[1];
                b = Library.AddSongFile(System.IO.Path.GetFullPath(FileName));
                Console.WriteLine($"Adding File {FileName} ({System.IO.Path.GetFullPath(FileName)}) returned {b}");
            }
            else if (Command.StartsWith("copy"))
            {
                CopyFiles(Command);
            }
            else if (Command.StartsWith("exit"))
            {
                Environment.Exit(0);
            }
        }

        // TODO -- We only handle "copy *.* <dest>" right now (and dest can't have spaces in it).
        private static void CopyFiles(string Command)
        {
            string[] Operands = Command.Split(' ');

            if (Operands.Length != 3)
            {
                Console.WriteLine($"Error: Invalid copy command ({Command})");
            }

            for (int i = 0; i < Operands.Length; ++i)
            {
                Console.WriteLine($"Operands[{i}] => {Operands[i]}");
            }

            int EnumerationContext = 0;
            List<IMetaData> Files = Context.GetItems(ref EnumerationContext, int.MaxValue, Operands[1]); // TODO -- Do pages and also need to verify Operands[1]
            foreach (var File in Files)
            {
                Console.WriteLine($"Copying '{File.FileName}' to '{Operands[2]}'");

                if (File.FileName == null)
                {
                    //
                    // This can happen if the file was deleted out from under itunes.
                    //
                    Console.WriteLine($"ERROR: File {File.LongDescription} is missing!");
                    continue;
                }

                string DestinationFile = System.IO.Path.Combine(Operands[2], System.IO.Path.GetFileName(File.FileName));

                if (System.IO.File.Exists(DestinationFile))
                {
                    // TODO -- Warn in verbose mode.
                    continue;
                }

                //
                // TODO -- Mirror directory structure (flag?)
                //
                System.IO.File.Copy(File.FileName, DestinationFile);
            }
        }

        private static string BuildPrompt()
        {
            IDirectoryContext TempContext = Context;
            StringBuilder sb = new StringBuilder();

            do
            {
                sb.Insert(0, $"\\{TempContext.Name}");
                TempContext = TempContext.Parent;
            } while (TempContext != null);

            sb.Append("> ");
            return sb.ToString();
        }
    }
}
