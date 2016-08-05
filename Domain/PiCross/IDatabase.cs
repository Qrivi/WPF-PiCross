using System;
using System.Collections.Generic;

namespace PiCross
{
    internal interface IDatabase
    {
        IPuzzleDatabase Puzzles { get; }

        IPlayerDatabase Players { get; }
    }

    internal interface IPuzzleDatabase
    {
        IEnumerable<IPuzzleDatabaseEntry> Entries { get; }

        IPuzzleDatabaseEntry this[int id] { get; }

        IPuzzleDatabaseEntry Create(Puzzle puzzle, string author);
    }

    internal interface IPuzzleDatabaseEntry
    {
        int UID { get; }

        Puzzle Puzzle { get; set; }

        string Author { get; set; }
    }

    internal interface IPlayerDatabase
    {
        IPlayerProfileData this[string name] { get; }

        IList<string> PlayerNames { get; }

        IPlayerProfileData CreateNewProfile(string name);
    }

    internal interface IPlayerProfileData
    {
        IPlayerPuzzleData this[int id] { get; }

        IEnumerable<int> EntryUIDs { get; }

        string Name { get; }
    }

    internal interface IPlayerPuzzleData
    {
        TimeSpan? BestTime { get; set; }
    }
}