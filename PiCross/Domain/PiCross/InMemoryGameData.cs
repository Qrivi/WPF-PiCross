using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using PiCross;
using Utility;

namespace PiCross
{
    internal class InMemoryGameData : IGameData
    {
        private readonly InMemoryPuzzleLibrary library;

        private readonly InMemoryPlayerDatabase playerDatabase;

        public InMemoryGameData(InMemoryPuzzleLibrary library, InMemoryPlayerDatabase playerDatabase)
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

        IPuzzleLibrary IGameData.PuzzleLibrary
        {
            get
            {
                return Library;
            }
        }

        IPlayerDatabase IGameData.PlayerDatabase
        {
            get
            {
                return PlayerDatabase;
            }
        }

        public InMemoryPuzzleLibrary Library
        {
            get
            {
                return library;
            }
        }

        public InMemoryPlayerDatabase PlayerDatabase
        {
            get
            {
                return playerDatabase;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryGameData );
        }

        public bool Equals(InMemoryGameData gameData)
        {
            return gameData != null && library.Equals( gameData.library ) && playerDatabase.Equals( gameData.playerDatabase );
        }

        public override int GetHashCode()
        {
            return library.GetHashCode() ^ playerDatabase.GetHashCode();
        }
    }

    internal class InMemoryPuzzleLibrary : IPuzzleLibrary
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

        IList<IPuzzleLibraryEntry> IPuzzleLibrary.Entries
        {
            get
            {
                return Entries.Cast<IPuzzleLibraryEntry>().ToList();
            }
        }

        public IList<InMemoryPuzzleLibraryEntry> Entries
        {
            get
            {
                return entries.AsReadOnly();
            }
        }

        public InMemoryPuzzleLibraryEntry GetEntryWithId( int id )
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

        IPuzzleLibraryEntry IPuzzleLibrary.Create( Puzzle puzzle, string author )
        {
            return Create( puzzle, author );
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

        public IDictionary<int, IPuzzleLibraryEntry> ToDictionary()
        {
            var result = new Dictionary<int, IPuzzleLibraryEntry>();

            foreach ( var entry in this.entries )
            {
                result[entry.UID] = entry;
            }

            return result;
        }
    }

    internal class InMemoryPuzzleLibraryEntry : IPuzzleLibraryEntry, IComparable<InMemoryPuzzleLibraryEntry>
    {
        private readonly int uid;

        private Puzzle puzzle;

        private readonly string author;

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

        public string Author { get { return author; } }

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

    internal class InMemoryPlayerDatabase : IPlayerDatabase
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

        IPlayerProfile IPlayerDatabase.this[string name]
        {
            get
            {
                return this[name];
            }
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

        IPlayerProfile IPlayerDatabase.CreateNewProfile( string name )
        {
            return CreateNewProfile( name );
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

        public void DeleteProfile( string name )
        {
            if ( name == null )
            {
                throw new ArgumentNullException( "name" );
            }
            else if ( !playerProfiles.ContainsKey( name ) )
            {
                throw new ArgumentException( "No player with name " + name );
            }
            else
            {
                this.playerProfiles.Remove( name );
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
    }

    internal class InMemoryPlayerProfile : IPlayerProfile
    {
        private readonly string name;

        private readonly InMemoryPlayerPuzzleInformation puzzleInformation;

        public InMemoryPlayerProfile( string name )
        {
            this.name = name;
            puzzleInformation = new InMemoryPlayerPuzzleInformation();
        }

        IPlayerPuzzleInformation IPlayerProfile.PuzzleInformation
        {
            get
            {
                return PuzzleInformation;
            }
        }

        public InMemoryPlayerPuzzleInformation PuzzleInformation
        {
            get { return puzzleInformation; }
        }

        public string Name
        {
            get { return name; }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as InMemoryPlayerProfile );
        }

        public bool Equals( InMemoryPlayerProfile playerProfile )
        {
            return playerProfile != null && name == playerProfile.name && puzzleInformation.Equals( playerProfile.puzzleInformation );
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }

    internal class InMemoryPlayerPuzzleInformation : IPlayerPuzzleInformation
    {
        private readonly Dictionary<int, InMemoryPlayerPuzzleInformationEntry> entries;

        public InMemoryPlayerPuzzleInformation()
        {
            this.entries = new Dictionary<int, InMemoryPlayerPuzzleInformationEntry>();
        }

        public IEnumerable<int> EntryUIDs
        {
            get
            {
                return entries.Keys;
            }
        }

        IPlayerPuzzleInformationEntry IPlayerPuzzleInformation.this[IPuzzleLibraryEntry libraryEntry]
        {
            get
            {
                return this[(InMemoryPuzzleLibraryEntry) libraryEntry];
            }
        }

        public InMemoryPlayerPuzzleInformationEntry this[InMemoryPuzzleLibraryEntry libraryEntry]
        {
            get
            {
                return this[libraryEntry.UID];
            }
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
            return Equals( obj as InMemoryPlayerPuzzleInformation );
        }

        public bool Equals( InMemoryPlayerPuzzleInformation playerPuzzleInformation )
        {
            if ( playerPuzzleInformation == null )
            {
                return false;
            }
            else
            {
                var ids = new HashSet<int>( this.entries.Keys.Concat( playerPuzzleInformation.entries.Keys ) );

                return ids.All( id => this[id].Equals( playerPuzzleInformation[id] ) );
            }
        }

        public override int GetHashCode()
        {
            return this.entries.GetHashCode();
        }
    }

    public class InMemoryPlayerPuzzleInformationEntry : IPlayerPuzzleInformationEntry
    {
        private readonly Cell<TimeSpan?> bestTime;

        public InMemoryPlayerPuzzleInformationEntry()
        {
            this.bestTime = Cell.Create<TimeSpan?>( null );
        }

        public Cell<TimeSpan?> BestTime
        {
            get
            {
                return bestTime;
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
