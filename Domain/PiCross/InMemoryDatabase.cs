﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Utility;

namespace PiCross
{
    internal class InMemoryDatabase : IDatabase
    {
        public InMemoryDatabase(PuzzleLibrary library, PlayerDatabase playerDatabase)
        {
            if (library == null)
            {
                throw new ArgumentNullException("library");
            }
            if (playerDatabase == null)
            {
                throw new ArgumentNullException("playerDatabase");
            }
            Puzzles = library;
            Players = playerDatabase;
        }

        public PuzzleLibrary Puzzles { get; }

        public PlayerDatabase Players { get; }

        IPuzzleDatabase IDatabase.Puzzles
        {
            get { return Puzzles; }
        }

        IPlayerDatabase IDatabase.Players
        {
            get { return Players; }
        }

        public static InMemoryDatabase CreateEmpty()
        {
            var puzzles = PuzzleLibrary.CreateEmpty();
            var players = PlayerDatabase.CreateEmpty();

            return new InMemoryDatabase(puzzles, players);
        }

        public static InMemoryDatabase ReadFromArchive(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    using (var archive = new GameDataArchive(zipArchive))
                    {
                        return ReadFromArchive(archive);
                    }
                }
            }
        }

        public static InMemoryDatabase ReadFromArchive(IGameDataArchive archive)
        {
            var gameData = CreateEmpty();

            foreach (var playerName in archive.PlayerNames)
            {
                var profile = archive.ReadPlayerProfile(playerName);
                gameData.Players.AddProfile(profile);
            }

            foreach (var uid in archive.PuzzleLibraryUIDs)
            {
                var entry = archive.ReadPuzzleLibraryEntry(uid);
                gameData.Puzzles.Add(entry);
            }

            return gameData;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InMemoryDatabase);
        }

        public bool Equals(InMemoryDatabase gameData)
        {
            return gameData != null && Puzzles.Equals(gameData.Puzzles) && Players.Equals(gameData.Players);
        }

        public override int GetHashCode()
        {
            return Puzzles.GetHashCode() ^ Players.GetHashCode();
        }

        public class PuzzleLibrary : IPuzzleDatabase
        {
            private readonly List<PuzzleLibraryEntry> entries;

            private int nextUID;

            private PuzzleLibrary()
            {
                entries = new List<PuzzleLibraryEntry>();
                nextUID = 0;
            }

            public IList<PuzzleLibraryEntry> Entries
            {
                get { return entries.AsReadOnly(); }
            }

            public PuzzleLibraryEntry this[int id]
            {
                get
                {
                    var result = entries.Find(entry => entry.UID == id);

                    if (result == null)
                    {
                        throw new ArgumentException("No entry found");
                    }
                    return result;
                }
            }

            IEnumerable<IPuzzleDatabaseEntry> IPuzzleDatabase.Entries
            {
                get { return Entries; }
            }

            IPuzzleDatabaseEntry IPuzzleDatabase.this[int id]
            {
                get { return this[id]; }
            }

            IPuzzleDatabaseEntry IPuzzleDatabase.Create(Puzzle puzzle, string author)
            {
                return Create(puzzle, author);
            }

            public static PuzzleLibrary CreateEmpty()
            {
                return new PuzzleLibrary();
            }

            public PuzzleLibraryEntry Create(Puzzle puzzle, string author)
            {
                var uid = nextUID++;

                Debug.Assert(!ContainsEntryWithUID(uid));

                var newEntry = new PuzzleLibraryEntry(uid, puzzle, author);

                entries.Add(newEntry);

                return newEntry;
            }

            public void Add(PuzzleLibraryEntry libraryEntry)
            {
                if (libraryEntry == null)
                {
                    throw new ArgumentNullException("libraryEntry");
                }
                if (ContainsEntryWithUID(libraryEntry.UID))
                {
                    throw new ArgumentException();
                }
                entries.Add(libraryEntry);
                nextUID = Math.Max(nextUID, libraryEntry.UID + 1);
            }

            private bool ContainsEntryWithUID(int uid)
            {
                return entries.Any(entry => entry.UID == uid);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as PuzzleLibrary);
            }

            public bool Equals(PuzzleLibrary library)
            {
                if (library == null)
                {
                    return false;
                }
                if (entries.Count != library.entries.Count)
                {
                    return false;
                }
                return Enumerable.Range(0, entries.Count).All(i => entries[i].Equals(library.entries[i]));
            }

            public override int GetHashCode()
            {
                return entries.Select(x => x.GetHashCode()).Aggregate((acc, n) => acc ^ n);
            }
        }

        internal class PuzzleLibraryEntry : IPuzzleDatabaseEntry
        {
            public PuzzleLibraryEntry(int uid, Puzzle puzzle, string author)
            {
                UID = uid;
                Puzzle = puzzle;
                Author = author;
            }

            public int UID { get; }

            public Puzzle Puzzle { get; set; }

            public string Author { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as PuzzleLibraryEntry);
            }

            public bool Equals(PuzzleLibraryEntry that)
            {
                return UID == that.UID;
            }

            public override int GetHashCode()
            {
                return UID.GetHashCode();
            }

            public int CompareTo(PuzzleLibraryEntry other)
            {
                return UID.CompareTo(other.UID);
            }
        }

        internal class PlayerDatabase : IPlayerDatabase
        {
            private readonly Dictionary<string, PlayerProfile> playerProfiles;

            private PlayerDatabase()
            {
                playerProfiles = new Dictionary<string, PlayerProfile>();
            }

            public PlayerProfile this[string name]
            {
                get
                {
                    if (!IsValidPlayerName(name))
                    {
                        throw new ArgumentException("Invalid name");
                    }
                    return playerProfiles[name];
                }
            }

            public IList<string> PlayerNames
            {
                get
                {
                    return (from profile in playerProfiles
                        let name = profile.Key
                        orderby name ascending
                        select name).ToList();
                }
            }

            IPlayerProfileData IPlayerDatabase.this[string name]
            {
                get { return this[name]; }
            }

            IPlayerProfileData IPlayerDatabase.CreateNewProfile(string name)
            {
                return CreateNewProfile(name);
            }

            public static PlayerDatabase CreateEmpty()
            {
                return new PlayerDatabase();
            }

            public bool IsValidPlayerName(string name)
            {
                return !string.IsNullOrWhiteSpace(name);
            }

            public PlayerProfile CreateNewProfile(string name)
            {
                if (!IsValidPlayerName(name))
                {
                    throw new ArgumentException("Invalid name");
                }
                if (playerProfiles.ContainsKey(name))
                {
                    throw new ArgumentException("Player already exists");
                }
                var profile = new PlayerProfile(name);

                AddToDictionary(profile);

                return profile;
            }

            public void AddProfile(PlayerProfile profile)
            {
                if (playerProfiles.ContainsKey(profile.Name))
                {
                    throw new ArgumentException("Player with same name already exists");
                }
                AddToDictionary(profile);
            }

            private void AddToDictionary(PlayerProfile profile)
            {
                playerProfiles[profile.Name] = profile;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as PlayerDatabase);
            }

            public bool Equals(PlayerDatabase playerDatabase)
            {
                return playerDatabase != null && playerProfiles.EqualItems(playerDatabase.playerProfiles);
            }

            public override int GetHashCode()
            {
                return playerProfiles.GetHashCode();
            }
        }

        internal class PlayerProfile : IPlayerProfileData
        {
            private readonly Dictionary<int, PlayerPuzzleInformationEntry> entries;

            public PlayerProfile(string name)
            {
                Name = name;
                entries = new Dictionary<int, PlayerPuzzleInformationEntry>();
            }

            public PlayerPuzzleInformationEntry this[int id]
            {
                get
                {
                    if (!entries.ContainsKey(id))
                    {
                        entries[id] = new PlayerPuzzleInformationEntry();
                    }

                    return entries[id];
                }
            }

            public string Name { get; }

            IPlayerPuzzleData IPlayerProfileData.this[int id]
            {
                get { return this[id]; }
            }


            public IEnumerable<int> EntryUIDs
            {
                get { return entries.Keys; }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as PlayerProfile);
            }

            public bool Equals(PlayerProfile playerProfile)
            {
                return Name == playerProfile.Name;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
        }

        public class PlayerPuzzleInformationEntry : IPlayerPuzzleData
        {
            public PlayerPuzzleInformationEntry()
            {
                BestTime = null;
            }

            public TimeSpan? BestTime { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as PlayerPuzzleInformationEntry);
            }

            public bool Equals(PlayerPuzzleInformationEntry entry)
            {
                return entry != null && BestTime.Equals(entry.BestTime);
            }

            public override int GetHashCode()
            {
                return BestTime.GetHashCode();
            }
        }
    }
}