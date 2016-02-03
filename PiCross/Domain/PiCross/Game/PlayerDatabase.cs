using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using PiCross.Game;
using PiCross.Facade.IO;
using Utility;

namespace PiCross.Game
{
    public class PlayerDatabase : IPlayerDatabase
    {
        private readonly Dictionary<string, PlayerProfile> playerProfiles;

        private readonly List<string> names; // TODO Remove this

        public PlayerDatabase()
        {
            playerProfiles = new Dictionary<string, PlayerProfile>();
            names = new List<string>();
        }

        IPlayerProfile IPlayerDatabase.this[string name]
        {
            get
            {
                return this[name];
            }
        }

        public PlayerProfile this[string name]
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

        public PlayerProfile CreateNewProfile( string name )
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
                var profile = new PlayerProfile( name );

                AddToDictionary( profile );
                AddToNames( profile.Name );

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
                this.names.Remove( name );
            }
        }

        private void AddToDictionary( PlayerProfile profile )
        {
            playerProfiles[profile.Name] = profile;
        }

        private void AddToNames( string name )
        {
            var index = 0;

            while ( index < this.names.Count && name.CompareTo( this.names[index] ) > 0 )
            {
                index++;
            }

            names.Insert( index, name );
        }

        public IList<string> PlayerNames
        {
            get
            {
                return names;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as PlayerDatabase );
        }

        public bool Equals( PlayerDatabase playerDatabase )
        {
            return playerDatabase != null && playerProfiles.EqualItems( playerDatabase.playerProfiles );
        }

        public override int GetHashCode()
        {
            return playerProfiles.GetHashCode();
        }
    }

    public class PlayerProfile : IPlayerProfile
    {
        private readonly string name;

        private readonly PlayerPuzzleInformation puzzleInformation;

        public PlayerProfile( string name )
        {
            this.name = name;
            puzzleInformation = new PlayerPuzzleInformation();
        }

        IPlayerPuzzleInformation IPlayerProfile.PuzzleInformation
        {
            get
            {
                return PuzzleInformation;
            }
        }

        public PlayerPuzzleInformation PuzzleInformation
        {
            get { return puzzleInformation; }
        }

        public string Name
        {
            get { return name; }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as PlayerProfile );
        }

        public bool Equals( PlayerProfile playerProfile )
        {
            return playerProfile != null && name == playerProfile.name && puzzleInformation.Equals( playerProfile.puzzleInformation );
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }

    public class PlayerPuzzleInformation : IPlayerPuzzleInformation
    {
        private readonly Dictionary<LibraryEntry, PlayerPuzzleInformationEntry> entries;

        public PlayerPuzzleInformation()
        {
            this.entries = new Dictionary<LibraryEntry, PlayerPuzzleInformationEntry>();
        }

        IPlayerPuzzleInformationEntry IPlayerPuzzleInformation.this[ILibraryEntry libraryEntry]
        {
            get
            {
                return this[(LibraryEntry) libraryEntry];
            }
        }

        public PlayerPuzzleInformationEntry this[LibraryEntry libraryEntry]
        {
            get
            {
                if ( !entries.ContainsKey( libraryEntry ) )
                {
                    entries[libraryEntry] = new PlayerPuzzleInformationEntry();
                }

                return entries[libraryEntry];
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as PlayerPuzzleInformation );
        }

        public bool Equals( PlayerPuzzleInformation playerPuzzleInformation )
        {
            if ( playerPuzzleInformation == null )
            {
                return false;
            }
            else
            {
                var libraryEntries = new HashSet<LibraryEntry>( this.entries.Keys.Concat(playerPuzzleInformation.entries.Keys) );

                return libraryEntries.All( entry => this[entry].Equals( playerPuzzleInformation[entry] ) );
            }
        }        

        public override int GetHashCode()
        {
            return this.entries.GetHashCode();
        }
    }

    public class PlayerPuzzleInformationEntry : IPlayerPuzzleInformationEntry
    {
        private readonly Cell<TimeSpan?> bestTime;

        public PlayerPuzzleInformationEntry()
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
            return Equals( obj as PlayerPuzzleInformationEntry );
        }

        public bool Equals(PlayerPuzzleInformationEntry  entry)
        {
            return entry != null && bestTime.Equals( entry.bestTime );
        }

        public override int GetHashCode()
        {
            return bestTime.GetHashCode();
        }
    }
}
