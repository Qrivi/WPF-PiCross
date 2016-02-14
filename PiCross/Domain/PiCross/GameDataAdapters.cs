using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    internal class GameDataAdapter : IGameData
    {
        private readonly IDatabase database;

        public GameDataAdapter( IDatabase database )
        {
            this.database = database;
        }

        public IPuzzleLibrary PuzzleLibrary
        {
            get { return new PuzzleLibraryAdapter( database.PuzzleDatabase ); }
        }

        public IPlayerLibrary PlayerDatabase
        {
            get { return new PlayerDatabaseAdapter( database.PlayerDatabase ); }
        }
    }

    internal class PuzzleLibraryAdapter : IPuzzleLibrary
    {
        private readonly IPuzzleDatabase puzzles;

        public PuzzleLibraryAdapter( IPuzzleDatabase puzzles )
        {
            this.puzzles = puzzles;
        }

        public IEnumerable<IPuzzleLibraryEntry> Entries
        {
            get
            {
                return from entry in puzzles.Entries
                       select new PuzzleLibraryEntryAdapter( entry );
            }
        }

        public IPuzzleLibraryEntry Create( Puzzle puzzle, string author )
        {
            return new PuzzleLibraryEntryAdapter( puzzles.Create( puzzle, author ) );
        }
    }

    internal class PuzzleLibraryEntryAdapter : IPuzzleLibraryEntry
    {
        private readonly IPuzzleDatabaseEntry entry;

        public PuzzleLibraryEntryAdapter( IPuzzleDatabaseEntry entry )
        {
            this.entry = entry;
        }

        public int UID
        {
            get
            {
                return entry.UID;
            }
        }

        public Puzzle Puzzle
        {
            get
            {
                return entry.Puzzle;
            }
            set
            {
                entry.Puzzle = value;
            }
        }

        public string Author
        {
            get
            {
                return entry.Author;
            }
            set
            {
                entry.Author = value;
            }
        }
    }

    internal class PlayerDatabaseAdapter : IPlayerLibrary
    {
        private readonly IPlayerDatabase database;

        public PlayerDatabaseAdapter( IPlayerDatabase database )
        {
            this.database = database;
        }

        public IPlayerProfile this[string name]
        {
            get { return new PlayerProfileAdapter( database[name] ); }
        }

        public IPlayerProfile CreateNewProfile( string name )
        {
            return new PlayerProfileAdapter( database.CreateNewProfile( name ) );
        }

        public bool IsValidPlayerName( string name )
        {
            throw new NotImplementedException(); // TODO
        }

        public IList<string> PlayerNames
        {
            get { return database.PlayerNames; }
        }
    }

    internal class PlayerProfileAdapter : IPlayerProfile
    {
        private readonly IPlayerProfileData data;

        public PlayerProfileAdapter( IPlayerProfileData data )
        {
            this.data = data;
        }

        public IPlayerPuzzleInformationEntry this[IPuzzleLibraryEntry libraryEntry]
        {
            get
            {
                var entry = ( (PuzzleLibraryEntryAdapter) libraryEntry );

                return new PlayerPuzzleInformationEntryAdapter( data[entry.UID] );
            }
        }

        public string Name
        {
            get { return data.Name; }
        }
    }

    internal class PlayerPuzzleInformationEntryAdapter : IPlayerPuzzleInformationEntry
    {
        private readonly IPlayerPuzzleData data;

        public PlayerPuzzleInformationEntryAdapter( IPlayerPuzzleData data )
        {
            this.data = data;
        }

        public TimeSpan? BestTime
        {
            get
            {
                return data.BestTime;
            }
            set
            {
                data.BestTime = value;
            }
        }
    }
}
