﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using PiCross;
using Utility;

namespace PiCross
{
    internal class InMemoryGameData : IDatabase
    {
        private readonly InMemoryPuzzleLibrary library;

        private readonly InMemoryPlayerDatabase playerDatabase;

        public InMemoryGameData( InMemoryPuzzleLibrary library, InMemoryPlayerDatabase playerDatabase )
        {
            if ( library == null )
            {
                throw new ArgumentNullException( "library" );
            }
            else if ( playerDatabase == null )
            {
                throw new ArgumentNullException( "playerDatabase" );
            }
            else
            {
                this.library = library;
                this.playerDatabase = playerDatabase;
            }
        }

        IPuzzleDatabase IDatabase.PuzzleDatabase
        {
            get
            {
                return PuzzleDatabase;
            }
        }

        public InMemoryPuzzleLibrary PuzzleDatabase
        {
            get
            {
                return this.library;
            }
        }

        IPlayerDatabase2 IDatabase.PlayerDatabase
        {
            get
            {
                return PlayerDatabase;
            }
        }

        public InMemoryPlayerDatabase PlayerDatabase
        {
            get
            {
                return this.playerDatabase;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryGameData );
        }

        public bool Equals( InMemoryGameData gameData )
        {
            return gameData != null && library.Equals( gameData.library ) && playerDatabase.Equals( gameData.playerDatabase );
        }

        public override int GetHashCode()
        {
            return library.GetHashCode() ^ playerDatabase.GetHashCode();
        }
    }

    internal class InMemoryPuzzleLibrary : IPuzzleDatabase
    {
        private readonly List<InMemoryPuzzleLibraryEntry> entries;

        private int nextUID;

        public static InMemoryPuzzleLibrary CreateEmpty()
        {
            return new InMemoryPuzzleLibrary();
        }

        private InMemoryPuzzleLibrary()
        {
            this.entries = new List<InMemoryPuzzleLibraryEntry>();
            nextUID = 0;
        }

        public IList<InMemoryPuzzleLibraryEntry> Entries
        {
            get
            {
                return entries.AsReadOnly();
            }
        }

        public InMemoryPuzzleLibraryEntry this[int id]
        {
            get
            {
                var result = entries.Find( entry => entry.UID == id );

                if ( result == null )
                {
                    throw new ArgumentException( "No entry found" );
                }
                else
                {
                    return result;
                }
            }
        }

        public InMemoryPuzzleLibraryEntry Create( Puzzle puzzle, string author )
        {
            var newEntry = new InMemoryPuzzleLibraryEntry( nextUID++, puzzle, author );

            entries.Add( newEntry );

            return newEntry;
        }

        public void Add( InMemoryPuzzleLibraryEntry libraryEntry )
        {
            if ( libraryEntry == null )
            {
                throw new ArgumentNullException( "libraryEntry" );
            }
            else if ( ContainsEntryWithUID( libraryEntry.UID ) )
            {
                throw new ArgumentException();
            }
            else
            {
                this.entries.Add( libraryEntry );
            }
        }

        private bool ContainsEntryWithUID( int uid )
        {
            return entries.Any( entry => entry.UID == uid );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryPuzzleLibrary );
        }

        public bool Equals( InMemoryPuzzleLibrary library )
        {
            if ( library == null )
            {
                return false;
            }
            else
            {
                if ( this.entries.Count != library.entries.Count )
                {
                    return false;
                }
                else
                {
                    return Enumerable.Range( 0, this.entries.Count ).All( i => entries[i].Equals( library.entries[i] ) );
                }
            }
        }

        public override int GetHashCode()
        {
            return entries.Select( x => x.GetHashCode() ).Aggregate( ( acc, n ) => acc ^ n );
        }

        //public IDictionary<int, IPuzzleLibraryEntry> ToDictionary()
        //{
        //    var result = new Dictionary<int, IPuzzleLibraryEntry>();

        //    foreach ( var entry in this.entries )
        //    {
        //        result[entry.UID] = entry;
        //    }

        //    return result;
        //}

        IEnumerable<IPuzzleDatabaseEntry> IPuzzleDatabase.Entries
        {
            get
            {
                return Entries;
            }
        }

        IPuzzleDatabaseEntry IPuzzleDatabase.this[int id]
        {
            get
            {
                return this[id];
            }
        }

        IPuzzleDatabaseEntry IPuzzleDatabase.Create( Puzzle puzzle, string author )
        {
            return Create( puzzle, author );
        }

        void IPuzzleDatabase.Add( IPuzzleDatabaseEntry libraryEntry )
        {
            Add( (InMemoryPuzzleLibraryEntry) libraryEntry );
        }
    }

    internal class InMemoryPuzzleLibraryEntry : IPuzzleDatabaseEntry
    {
        private readonly int uid;

        private Puzzle puzzle;

        private string author;

        public InMemoryPuzzleLibraryEntry( int uid, Puzzle puzzle, string author )
        {
            this.uid = uid;
            this.puzzle = puzzle;
            this.author = author;
        }

        public int UID
        {
            get
            {
                return uid;
            }
        }

        public Puzzle Puzzle
        {
            get
            {
                return puzzle;
            }
            set
            {
                this.puzzle = value;
            }
        }

        public string Author
        {
            get { return author; }
            set { author = value; }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryPuzzleLibraryEntry );
        }

        public bool Equals( InMemoryPuzzleLibraryEntry that )
        {
            return this.uid == that.uid;
        }

        public override int GetHashCode()
        {
            return uid.GetHashCode();
        }

        public int CompareTo( InMemoryPuzzleLibraryEntry other )
        {
            return this.uid.CompareTo( other.uid );
        }
    }

    internal class InMemoryPlayerDatabase : IPlayerDatabase2
    {
        private readonly Dictionary<string, InMemoryPlayerProfile> playerProfiles;

        public static InMemoryPlayerDatabase CreateEmpty()
        {
            return new InMemoryPlayerDatabase();
        }

        private InMemoryPlayerDatabase()
        {
            playerProfiles = new Dictionary<string, InMemoryPlayerProfile>();
        }

        public InMemoryPlayerProfile this[string name]
        {
            get
            {
                if ( !IsValidPlayerName( name ) )
                {
                    throw new ArgumentException( "Invalid name" );
                }
                else
                {
                    return playerProfiles[name];
                }
            }
        }

        public bool IsValidPlayerName( string name )
        {
            return !string.IsNullOrWhiteSpace( name );
        }

        public InMemoryPlayerProfile CreateNewProfile( string name )
        {
            if ( !IsValidPlayerName( name ) )
            {
                throw new ArgumentException( "Invalid name" );
            }
            else if ( playerProfiles.ContainsKey( name ) )
            {
                throw new ArgumentException( "Player already exists" );
            }
            else
            {
                var profile = new InMemoryPlayerProfile( name );

                AddToDictionary( profile );

                return profile;
            }
        }

        private void AddToDictionary( InMemoryPlayerProfile profile )
        {
            playerProfiles[profile.Name] = profile;
        }

        public IList<string> PlayerNames
        {
            get
            {
                return ( from profile in this.playerProfiles
                         let name = profile.Key
                         orderby name ascending
                         select name ).ToList();
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryPlayerDatabase );
        }

        public bool Equals( InMemoryPlayerDatabase playerDatabase )
        {
            return playerDatabase != null && playerProfiles.EqualItems( playerDatabase.playerProfiles );
        }

        public override int GetHashCode()
        {
            return playerProfiles.GetHashCode();
        }

        IPlayerProfileData IPlayerDatabase2.this[string name]
        {
            get
            {
                return this[name];
            }
        }

        IPlayerProfileData IPlayerDatabase2.CreateNewProfile( string name )
        {
            return CreateNewProfile( name );
        }
    }

    internal class InMemoryPlayerProfile : IPlayerProfileData
    {
        private readonly string name;

        private readonly Dictionary<int, InMemoryPlayerPuzzleInformationEntry> entries;

        public InMemoryPlayerProfile( string name )
        {
            this.name = name;
            entries = new Dictionary<int, InMemoryPlayerPuzzleInformationEntry>();
        }

        public string Name
        {
            get { return name; }
        }

        public InMemoryPlayerPuzzleInformationEntry this[int id]
        {
            get
            {
                if ( !entries.ContainsKey( id ) )
                {
                    entries[id] = new InMemoryPlayerPuzzleInformationEntry();
                }

                return entries[id];
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryPlayerProfile );
        }

        public bool Equals( InMemoryPlayerProfile playerProfile )
        {
            throw new NotImplementedException(); // TODO
            // return playerProfile != null && name == playerProfile.name && puzzleInformation.Equals( playerProfile.puzzleInformation );
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        IPlayerPuzzleData IPlayerProfileData.this[int id]
        {
            get { return this[id]; }
        }


        public IEnumerable<int> EntryUIDs
        {
            get
            {
                return this.entries.Keys;
            }
        }
    }

    public class InMemoryPlayerPuzzleInformationEntry : IPlayerPuzzleData
    {
        private TimeSpan? bestTime;

        public InMemoryPlayerPuzzleInformationEntry()
        {
            this.bestTime = null;
        }

        public TimeSpan? BestTime
        {
            get
            {
                return bestTime;
            }
            set
            {
                bestTime = value;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryPlayerPuzzleInformationEntry );
        }

        public bool Equals( InMemoryPlayerPuzzleInformationEntry entry )
        {
            return entry != null && bestTime.Equals( entry.bestTime );
        }

        public override int GetHashCode()
        {
            return bestTime.GetHashCode();
        }
    }
}
