using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public class PlayerDatabase : IPlayerDatabase
    {
        private readonly Dictionary<string, PlayerProfile> playerProfiles;

        private readonly ObservableCollection<string> names;

        public PlayerDatabase()
        {
            playerProfiles = new Dictionary<string, PlayerProfile>();
            names = new ObservableCollection<string>();
        }

        public IPlayerProfile this[string name]
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
        
        public IPlayerProfile CreateNewProfile( string name )
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

        public void DeleteProfile(string name)
        {
            if ( name == null )
            {
                throw new ArgumentNullException( "name" );
            }
            else if ( !playerProfiles.ContainsKey(name))
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

        public ObservableCollection<string> PlayerNames
        {
            get
            {
                return names;
            }
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

        public IPlayerPuzzleInformation PuzzleInformation
        {
            get { return puzzleInformation; }
        }

        public string Name
        {
            get { return name; }
        }
    }

    public class PlayerPuzzleInformation : IPlayerPuzzleInformation
    {
        private readonly Dictionary<ILibraryEntry, IPlayerPuzzleInformationEntry> entries;

        public PlayerPuzzleInformation()
        {
            this.entries = new Dictionary<ILibraryEntry, IPlayerPuzzleInformationEntry>();
        }

        public IPlayerPuzzleInformationEntry this[ILibraryEntry libraryEntry]
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
    }
}
