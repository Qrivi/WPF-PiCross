using System;
using System.Collections.Generic;

namespace PiCross
{
    public interface IGameData
    {
        IPuzzleLibrary PuzzleLibrary { get; }

        IPlayerLibrary PlayerDatabase { get; }
    }

    /// <summary>
    ///     Player Library.
    /// </summary>
    public interface IPlayerLibrary
    {
        /// <summary>
        ///     Retrieves the profile of the player with the specified name.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        /// <returns>Player's profile.</returns>
        IPlayerProfile this[string name] { get; }

        /// <summary>
        ///     Enumerates all the players' names.
        /// </summary>
        IList<string> PlayerNames { get; }

        /// <summary>
        ///     Creates a new player profile.
        /// </summary>
        /// <param name="name">Player name.</param>
        /// <returns>The newly created player profile.</returns>
        IPlayerProfile CreateNewProfile(string name);

        /// <summary>
        ///     Checks if the specified name is valid.
        /// </summary>
        /// <param name="name">Name to be verified.</param>
        /// <returns>True if the name is valid, false otherwise.</returns>
        bool IsValidPlayerName(string name);
    }

    /// <summary>
    ///     Player's profile.
    /// </summary>
    public interface IPlayerProfile
    {
        /// <summary>
        ///     Fetches player-specific information about the given puzzle.
        /// </summary>
        /// <param name="libraryEntry">Puzzle.</param>
        /// <returns></returns>
        IPlayerPuzzleInformation this[IPuzzleLibraryEntry libraryEntry] { get; }

        /// <summary>
        ///     Name of the player whose profile this is.
        /// </summary>
        string Name { get; }
    }

    public interface IPlayerPuzzleInformation
    {
        TimeSpan? BestTime { get; set; }
    }

    public interface IPuzzleLibrary
    {
        IEnumerable<IPuzzleLibraryEntry> Entries { get; }

        IPuzzleLibraryEntry Create(Puzzle puzzle, string author);
    }

    public interface IPuzzleLibraryEntry
    {
        Puzzle Puzzle { get; set; }

        string Author { get; set; }
    }
}