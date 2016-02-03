using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross.Game;
using PiCross.Facade.IO;

namespace PiCross
{
    public class Library : ILibrary
    {
        private readonly List<LibraryEntry> entries;

        private int nextUID;

        public static Library CreateEmpty()
        {
            return new Library();
        }

        private Library()
        {
            this.entries = new List<LibraryEntry>();
            nextUID = 0;
        }

        IList<ILibraryEntry> ILibrary.Entries
        {
            get
            {
                return Entries.Cast<ILibraryEntry>().ToList();
            }
        }

        public IList<LibraryEntry> Entries
        {
            get
            {
                return entries.AsReadOnly();
            }
        }

        public LibraryEntry GetEntryWithId(int id)
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

        ILibraryEntry ILibrary.Create(Puzzle puzzle, string author)
        {
            return Create( puzzle, author );
        }

        public LibraryEntry Create( Puzzle puzzle, string author)
        {
            var newEntry = new LibraryEntry( nextUID++, puzzle, author );

            entries.Add( newEntry );

            return newEntry;
        }

        public void Add(LibraryEntry libraryEntry)
        {
            if ( libraryEntry == null )
            {
                throw new ArgumentNullException( "libraryEntry" );
            }
            else if ( ContainsEntryWithUID(libraryEntry.UID))
            {
                throw new ArgumentException();
            }
            else
            {
                this.entries.Add( libraryEntry );
            }
        }

        private bool ContainsEntryWithUID(int uid)
        {
            return entries.Any( entry => entry.UID == uid );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Library );
        }

        public bool Equals(Library library)
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

        public IDictionary<int, ILibraryEntry> ToDictionary()
        {
            var result = new Dictionary<int, ILibraryEntry>();

            foreach ( var entry in this.entries )
            {
                result[entry.UID] = entry;
            }

            return result;
        }
    }

    public class LibraryEntry : ILibraryEntry, IComparable<LibraryEntry>
    {
        private readonly int uid;

        private Puzzle puzzle;

        private readonly string author;

        public LibraryEntry( int uid, Puzzle puzzle, string author )
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
            return Equals( obj as LibraryEntry );
        }

        public bool Equals( LibraryEntry that )
        {
            return this.uid == that.uid;
        }

        public override int GetHashCode()
        {
            return uid.GetHashCode();
        }

        public int CompareTo( LibraryEntry other )
        {
            return this.uid.CompareTo( other.uid );
        }
    }
}
