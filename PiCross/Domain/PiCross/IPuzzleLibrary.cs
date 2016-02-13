using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross;

namespace PiCross
{
    public interface IPuzzleLibrary
    {
        IList<IPuzzleLibraryEntry> Entries { get; }

        IPuzzleLibraryEntry Create( Puzzle puzzle, string author );
    }

    public interface IPuzzleLibraryEntry
    {
        Puzzle Puzzle { get; set; }

        string Author { get; }
    }
}
