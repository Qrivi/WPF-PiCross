using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross.Game;

namespace PiCross.Facade
{
    public interface ILibrary
    {
        IList<ILibraryEntry> Entries { get; }

        ILibraryEntry Create( Puzzle puzzle, string author );
    }

    public interface ILibraryEntry
    {
        Puzzle Puzzle { get; set; }

        string Author { get; }
    }
}
