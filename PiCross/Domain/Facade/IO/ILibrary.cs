using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public interface ILibrary
    {
        ObservableCollection<ILibraryEntry> Entries { get; }
    }

    public interface ILibraryEntry
    {
        Puzzle Puzzle { get; }

        string Author { get; }
    }
}
