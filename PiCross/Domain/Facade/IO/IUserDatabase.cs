using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public interface IUserDatabase
    {
        IUserProfile GetUserProfile( string name );
    }

    public interface IUserProfile
    {
        IUserPuzzleInformation PuzzleInformation { get; }

        string Name { get; }
    }

    public interface IUserPuzzleInformation
    {
        IUserPuzzleInformationEntry this[Puzzle puzzle] { get; set; }
    }

    public interface IUserPuzzleInformationEntry
    {
        TimeSpan BestTime { get; }
    }
}
