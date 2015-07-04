using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public interface IPlayerDatabase
    {
        IPlayerProfile this[string name] { get; }

        IPlayerProfile CreateNewProfile( string name );

        bool IsValidPlayerName( string name );

        ObservableCollection<string> PlayerNames { get; }
    }

    public interface IPlayerProfile
    {
        IPlayerPuzzleInformation PuzzleInformation { get; }

        string Name { get; }
    }

    public interface IPlayerPuzzleInformation
    {
        IPlayerPuzzleInformationEntry this[Puzzle puzzle] { get; set; }
    }

    public interface IPlayerPuzzleInformationEntry
    {
        Cell<TimeSpan> BestTime { get; }
    }
}
