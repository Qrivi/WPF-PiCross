using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross
{
    internal class ArchiveDatabase : IDatabase
    {
        private readonly IGameDataArchive archive;

        public ArchiveDatabase( IGameDataArchive archive )
        {
            this.archive = archive;
        }

        public IPuzzleDatabase PuzzleDatabase
        {
            get
            {
                return new PuzzleDB( archive );
            }
        }

        public IPlayerDatabase PlayerDatabase
        {
            get { return new PlayerDB( archive ); }
        }

        private class PuzzleDB : IPuzzleDatabase // TODO Rename
        {
            private readonly IGameDataArchive archive;

            public PuzzleDB( IGameDataArchive archive )
            {
                this.archive = archive;
            }

            public IEnumerable<IPuzzleDatabaseEntry> Entries
            {
                get
                {
                    return from uid in archive.PuzzleLibraryUIDs
                           let entry = archive.ReadPuzzleLibraryEntry( uid )
                           select new PuzzleDatabaseEntry( archive, entry.UID, entry.Puzzle, entry.Author );
                }
            }

            public IPuzzleDatabaseEntry this[int id]
            {
                get
                {
                    var entry = archive.ReadPuzzleLibraryEntry( id );

                    return new PuzzleDatabaseEntry( archive, entry.UID, entry.Puzzle, entry.Author );
                }
            }

            private int GenerateUniqueUID()
            {
                var uids = archive.PuzzleLibraryUIDs.ToList();
                uids.Add( 0 );

                return uids.Max() + 1;
            }

            public IPuzzleDatabaseEntry Create( Puzzle puzzle, string author )
            {
                var uid = GenerateUniqueUID();
                var entry = new InMemoryPuzzleLibraryEntry( uid, puzzle, author );
                archive.UpdateLibraryEntry( entry );

                return new PuzzleDatabaseEntry( archive, uid, puzzle, author );
            }

            public void Add( IPuzzleDatabaseEntry libraryEntry )
            {
                throw new NotImplementedException();
            }
        }

        private class PuzzleDatabaseEntry : IPuzzleDatabaseEntry
        {
            private readonly IGameDataArchive archive;

            private readonly int uid;

            private Puzzle puzzle;

            private string author;

            public PuzzleDatabaseEntry( IGameDataArchive archive, int uid, Puzzle puzzle, string author )
            {
                this.archive = archive;
                this.uid = uid;
                this.puzzle = puzzle;
                this.author = author;
            }

            public int UID
            {
                get { return uid; }
            }

            public Puzzle Puzzle
            {
                get
                {
                    return puzzle;
                }
                set
                {
                    var entry = new InMemoryPuzzleLibraryEntry( uid, value, author );
                    archive.UpdateLibraryEntry( entry );
                    this.puzzle = value;
                }
            }

            public string Author
            {
                get
                {
                    return author;
                }
                set
                {
                    var entry = new InMemoryPuzzleLibraryEntry( uid, puzzle, value );
                    archive.UpdateLibraryEntry( entry );
                    this.author = value;
                }
            }
        }

        private class PlayerDB : IPlayerDatabase
        {
            private readonly IGameDataArchive archive;

            public PlayerDB( IGameDataArchive archive )
            {
                this.archive = archive;
            }

            public IPlayerProfileData this[string name]
            {
                get
                {
                    var profile = archive.ReadPlayerProfile( name );

                    return new PlayerProfileData( archive, profile.Name );
                }
            }

            public IPlayerProfileData CreateNewProfile( string name )
            {
                // TODO Check for duplicates

                var profile = new InMemoryPlayerProfile( name );
                archive.UpdatePlayerProfile( profile );

                return new PlayerProfileData( archive, name );
            }

            public IList<string> PlayerNames
            {
                get
                {
                    return archive.PlayerNames;
                }
            }
        }

        private class PlayerProfileData : IPlayerProfileData
        {
            private readonly IGameDataArchive archive;

            private readonly string name;

            public PlayerProfileData( IGameDataArchive archive, string name )
            {
                this.archive = archive;
                this.name = name;
            }

            public IPlayerPuzzleData this[int id]
            {
                get
                {
                    return new PlayerPuzzleData( archive, name, id );
                }
            }

            public IEnumerable<int> EntryUIDs
            {
                get
                {
                    var profile = archive.ReadPlayerProfile( name );
                    return profile.EntryUIDs;
                }
            }

            public string Name
            {
                get { return name; }
            }
        }

        private class PlayerPuzzleData : IPlayerPuzzleData
        {
            private readonly IGameDataArchive archive;

            private readonly string playerName;

            private readonly int uid;

            public PlayerPuzzleData( IGameDataArchive archive, string playerName, int uid )
            {
                this.archive = archive;
                this.playerName = playerName;
                this.uid = uid;
            }

            private InMemoryPlayerProfile ReadPlayerProfile()
            {
                return archive.ReadPlayerProfile( playerName );
            }

            private InMemoryPlayerPuzzleInformationEntry ReadInfo()
            {
                var profile = ReadPlayerProfile();

                return profile[uid];
            }

            public TimeSpan? BestTime
            {
                get
                {
                    return ReadInfo().BestTime;
                }
                set
                {
                    var profile = ReadPlayerProfile();
                    profile[uid].BestTime = value;

                    archive.UpdatePlayerProfile( profile );
                }
            }
        }
    }
}
