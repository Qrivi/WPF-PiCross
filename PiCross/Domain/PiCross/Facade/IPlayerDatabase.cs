using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using PiCross.Game;

namespace PiCross
{
    public interface IPlayerDatabase
    {
        IPlayerProfile this[string name] { get; }

        IPlayerProfile CreateNewProfile( string name );

        bool IsValidPlayerName( string name );

        IList<string> PlayerNames { get; }
    }

    public interface IPlayerProfile
    {
        IPlayerPuzzleInformation PuzzleInformation { get; }

        string Name { get; }
    }

    public interface IPlayerPuzzleInformation
    {
        IPlayerPuzzleInformationEntry this[IPuzzleLibraryEntry libraryEntry] { get; }
    }

    public interface IPlayerPuzzleInformationEntry
    {
        // TODO Simplify
        Cell<TimeSpan?> BestTime { get; }
    }
}
