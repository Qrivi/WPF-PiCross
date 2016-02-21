using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross
{
    internal class ArchivedGameData : IGameData
    {
        private readonly IGameDataArchive archive;

        private readonly InMemoryGameData gameData;

        public ArchivedGameData( IGameDataArchive archive )
        {
            this.archive = archive;
            this.gameData = InMemoryGameData.ReadFromArchive( archive );
        }

        public IPuzzleLibrary PuzzleLibrary
        {
            get
            {
                return new PuzzleLibraryImpl( archive, gameData.PuzzleDatabase );
            }
        }

        public IPlayerLibrary PlayerDatabase
        {
            get
            {
                return new PlayerLibrary( archive, gameData.PlayerDatabase );
            }
        }

        private class PlayerLibrary : IPlayerLibrary
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryPlayerDatabase players;

            public PlayerLibrary( IGameDataArchive archive, InMemoryPlayerDatabase players )
            {
                this.archive = archive;
                this.players = players;
            }

            public IPlayerProfile this[string name]
            {
                get
                {
                    return new PlayerProfile( archive, players[name] );
                }
            }

            public IPlayerProfile CreateNewProfile( string name )
            {
                var profile = players.CreateNewProfile( name );

                archive.UpdatePlayerProfile( profile );

                return new PlayerProfile( archive, profile );
            }

            public bool IsValidPlayerName( string name )
            {
                return this.players.IsValidPlayerName( name );
            }

            public IList<string> PlayerNames
            {
                get
                {
                    return this.players.PlayerNames;
                }
            }
        }

        private class PlayerProfile : IPlayerProfile
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryPlayerProfile profile;

            public PlayerProfile( IGameDataArchive archive, InMemoryPlayerProfile profile )
            {
                this.archive = archive;
                this.profile = profile;
            }

            public IPlayerPuzzleInformationEntry this[IPuzzleLibraryEntry libraryEntry]
            {
                get
                {
                    var entry = (PuzzleLibraryEntry) libraryEntry;
                    var info = profile[entry.UID];
                    return new PlayerPuzzleInformationEntry( archive, profile, info );
                }
            }

            public string Name
            {
                get { return profile.Name; }
            }
        }

        private class PlayerPuzzleInformationEntry : IPlayerPuzzleInformationEntry
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryPlayerProfile playerProfile;

            private readonly InMemoryPlayerPuzzleInformationEntry entry;

            public PlayerPuzzleInformationEntry(IGameDataArchive archive, InMemoryPlayerProfile profile, InMemoryPlayerPuzzleInformationEntry entry)
            {
                this.archive = archive;
                this.playerProfile = profile;
                this.entry = entry;
            }

            public TimeSpan? BestTime
            {
                get
                {
                    return this.entry.BestTime;
                }
                set
                {
                    this.entry.BestTime = value;
                    archive.UpdatePlayerProfile( playerProfile );
                }
            }
        }

        private class PuzzleLibraryImpl : IPuzzleLibrary // TODO Rename
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryPuzzleLibrary library;

            public PuzzleLibraryImpl( IGameDataArchive archive, InMemoryPuzzleLibrary library )
            {
                this.archive = archive;
                this.library = library;
            }

            public IEnumerable<IPuzzleLibraryEntry> Entries
            {
                get
                {
                    return from entry in library.Entries
                           select new PuzzleLibraryEntry( archive, entry );
                }
            }

            public IPuzzleLibraryEntry Create( Puzzle puzzle, string author )
            {
                var entry = this.library.Create( puzzle, author );
                archive.UpdateLibraryEntry( entry );
                return new PuzzleLibraryEntry( archive, entry );
            }
        }

        private class PuzzleLibraryEntry : IPuzzleLibraryEntry
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryPuzzleLibraryEntry entry;

            public PuzzleLibraryEntry( IGameDataArchive archive, InMemoryPuzzleLibraryEntry entry )
            {
                this.archive = archive;
                this.entry = entry;
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
                    UpdateArchive();
                }
            }

            public int UID
            {
                get
                {
                    return entry.UID;
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
                    UpdateArchive();
                }
            }

            private void UpdateArchive()
            {
                archive.UpdateLibraryEntry( entry );
            }

            public override bool Equals( object obj )
            {
                return Equals( obj as PuzzleLibraryEntry );
            }

            public bool Equals(PuzzleLibraryEntry entry)
            {
                return entry != null && this.UID == entry.UID;
            }

            public override int GetHashCode()
            {
                return UID;
            }
        }
    }
}
