using System;
using System.Collections.Generic;
using System.Linq;

namespace PiCross
{
    internal class ArchiveDatabase : IDatabase
    {
        private readonly IGameDataArchive archive;

        private readonly InMemoryDatabase database;

        public ArchiveDatabase(string path)
        {
            database = InMemoryDatabase.ReadFromArchive(path);
            archive = new AutoCloseGameDataArchive(path);
        }

        public IPuzzleDatabase Puzzles
        {
            get { return new PuzzleDatabase(archive, database.Puzzles); }
        }

        public IPlayerDatabase Players
        {
            get { return new PlayerDatabase(archive, database.Players); }
        }

        private class PuzzleDatabase : IPuzzleDatabase
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryDatabase.PuzzleLibrary puzzleLibrary;

            public PuzzleDatabase(IGameDataArchive archive, InMemoryDatabase.PuzzleLibrary puzzleLibrary)
            {
                this.archive = archive;
                this.puzzleLibrary = puzzleLibrary;
            }

            public IEnumerable<IPuzzleDatabaseEntry> Entries
            {
                get
                {
                    return from entry in puzzleLibrary.Entries
                        select new PuzzleDatabaseEntry(archive, entry);
                }
            }

            public IPuzzleDatabaseEntry this[int id]
            {
                get
                {
                    var entry = puzzleLibrary[id];

                    return new PuzzleDatabaseEntry(archive, entry);
                }
            }

            public IPuzzleDatabaseEntry Create(Puzzle puzzle, string author)
            {
                var entry = puzzleLibrary.Create(puzzle, author);
                archive.UpdateLibraryEntry(entry);

                return new PuzzleDatabaseEntry(archive, entry);
            }
        }

        private class PuzzleDatabaseEntry : IPuzzleDatabaseEntry
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryDatabase.PuzzleLibraryEntry entry;

            public PuzzleDatabaseEntry(IGameDataArchive archive, InMemoryDatabase.PuzzleLibraryEntry entry)
            {
                this.archive = archive;
                this.entry = entry;
            }

            public int UID
            {
                get { return entry.UID; }
            }

            public Puzzle Puzzle
            {
                get { return entry.Puzzle; }
                set
                {
                    entry.Puzzle = value;
                    archive.UpdateLibraryEntry(entry);
                }
            }

            public string Author
            {
                get { return entry.Author; }
                set
                {
                    entry.Author = value;
                    archive.UpdateLibraryEntry(entry);
                }
            }
        }

        private class PlayerDatabase : IPlayerDatabase
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryDatabase.PlayerDatabase playerDatabase;

            public PlayerDatabase(IGameDataArchive archive, InMemoryDatabase.PlayerDatabase playerDatabase)
            {
                this.archive = archive;
                this.playerDatabase = playerDatabase;
            }

            public IPlayerProfileData this[string name]
            {
                get
                {
                    var profile = archive.ReadPlayerProfile(name);

                    return new PlayerProfileData(archive, profile);
                }
            }

            public IPlayerProfileData CreateNewProfile(string name)
            {
                var profile = playerDatabase.CreateNewProfile(name);
                archive.UpdatePlayerProfile(profile);

                return new PlayerProfileData(archive, profile);
            }

            public IList<string> PlayerNames
            {
                get { return playerDatabase.PlayerNames; }
            }
        }

        private class PlayerProfileData : IPlayerProfileData
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryDatabase.PlayerProfile profile;

            public PlayerProfileData(IGameDataArchive archive, InMemoryDatabase.PlayerProfile profile)
            {
                this.archive = archive;
                this.profile = profile;
            }

            public IPlayerPuzzleData this[int id]
            {
                get { return new PlayerPuzzleData(archive, profile, id); }
            }

            public IEnumerable<int> EntryUIDs
            {
                get { return profile.EntryUIDs; }
            }

            public string Name
            {
                get { return profile.Name; }
            }
        }

        private class PlayerPuzzleData : IPlayerPuzzleData
        {
            private readonly IGameDataArchive archive;

            private readonly InMemoryDatabase.PlayerPuzzleInformationEntry entry;

            private readonly InMemoryDatabase.PlayerProfile profile;

            private readonly int uid;

            public PlayerPuzzleData(IGameDataArchive archive, InMemoryDatabase.PlayerProfile profile, int uid)
            {
                this.archive = archive;
                this.profile = profile;
                this.uid = uid;
                entry = profile[uid];
            }

            public TimeSpan? BestTime
            {
                get { return entry.BestTime; }
                set
                {
                    profile[uid].BestTime = value;

                    archive.UpdatePlayerProfile(profile);
                }
            }
        }
    }
}