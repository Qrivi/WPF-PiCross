using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    public interface IGameData
    {
        IPuzzleLibrary PuzzleLibrary { get; }

        IPlayerDatabase PlayerDatabase { get; }
    }

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
