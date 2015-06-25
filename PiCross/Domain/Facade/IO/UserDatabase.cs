using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public class UserDatabase : IUserDatabase
    {
        private readonly Dictionary<string, UserProfile> userProfiles;

        public UserDatabase()
        {
            userProfiles = new Dictionary<string, UserProfile>();
        }

        public IUserProfile this[string name]
        {
            get
            {
                if ( !IsValidName( name ) )
                {
                    throw new ArgumentException( "Invalid name" );
                }
                else
                {
                    return userProfiles[name];
                }
            }
        }

        private bool IsValidName( string name )
        {
            return !string.IsNullOrWhiteSpace( name );
        }

        public IUserProfile CreateNewProfile( string name )
        {
            if ( !IsValidName( name ) )
            {
                throw new ArgumentException( "Invalid name" );
            }
            else if ( userProfiles.ContainsKey( name ) )
            {
                throw new ArgumentException( "User already exists" );
            }
            else
            {
                var profile = new UserProfile( name );

                userProfiles[name] = profile;

                return profile;
            }
        }

        public ISet<string> UserNames
        {
            get
            {
                return new HashSet<string>( userProfiles.Keys );
            }
        }
    }

    public class UserProfile : IUserProfile
    {
        private readonly string name;

        private readonly UserPuzzleInformation puzzleInformation;

        public UserProfile( string name )
        {
            this.name = name;
            puzzleInformation = new UserPuzzleInformation();
        }

        public IUserPuzzleInformation PuzzleInformation
        {
            get { return puzzleInformation; }
        }

        public string Name
        {
            get { return name; }
        }
    }

    public class UserPuzzleInformation : IUserPuzzleInformation
    {
        private readonly Dictionary<Puzzle, IUserPuzzleInformationEntry> entries;

        public UserPuzzleInformation()
        {
            this.entries = new Dictionary<Puzzle, IUserPuzzleInformationEntry>();
        }

        public IUserPuzzleInformationEntry this[Puzzle puzzle]
        {
            get
            {
                return entries[puzzle];
            }
            set
            {
                entries[puzzle] = value;
            }
        }
    }

    public class UserPuzzleInformationEntry : IUserPuzzleInformationEntry
    {
        private readonly TimeSpan bestTime;

        public UserPuzzleInformationEntry( TimeSpan bestTime )
        {
            this.bestTime = bestTime;
        }

        public TimeSpan BestTime
        {
            get
            {
                return bestTime;
            }
        }
    }
}
