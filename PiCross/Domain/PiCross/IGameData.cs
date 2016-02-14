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

        IPlayerLibrary PlayerDatabase { get; }
    }

    public interface IPlayerLibrary
    {
        IPlayerProfile this[string name] { get; }

        IPlayerProfile CreateNewProfile( string name );

        bool IsValidPlayerName( string name );

        IList<string> PlayerNames { get; }
    }

    public interface IPlayerProfile
    {
        IPlayerPuzzleInformationEntry this[IPuzzleLibraryEntry libraryEntry] { get; }

        string Name { get; }
    }

    public interface IPlayerPuzzleInformationEntry
    {
        TimeSpan? BestTime { get; set; }
    }

    public interface IPuzzleLibrary
    {
        IEnumerable<IPuzzleLibraryEntry> Entries { get; }

        IPuzzleLibraryEntry Create( Puzzle puzzle, string author );
    }

    public interface IPuzzleLibraryEntry
    {
        Puzzle Puzzle { get; set; }

        string Author { get; set; }
    }
}
