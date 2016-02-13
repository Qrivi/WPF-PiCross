using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using PiCross;
using Utility;

namespace PiCross
{
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
