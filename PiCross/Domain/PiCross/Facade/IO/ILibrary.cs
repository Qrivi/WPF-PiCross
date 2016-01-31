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

namespace PiCross.Facade.IO
{
    public interface ILibrary
    {
        ObservableCollection<ILibraryEntry> Entries { get; }

        ILibraryEntry Create( Size size, string author );
    }

    public interface ILibraryEntry
    {
        Puzzle Puzzle { get; }

        string Author { get; }
    }
}
