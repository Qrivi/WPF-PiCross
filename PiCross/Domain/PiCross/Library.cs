using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross;

namespace PiCross
{
    internal class PuzzleLibrary : IPuzzleLibrary
    {
        private readonly List<PuzzleLibraryEntry> entries;

        private int nextUID;

        public static PuzzleLibrary CreateEmpty()
        {
            return new PuzzleLibrary();
        }

        private PuzzleLibrary()
        {
            this.entries = new List<PuzzleLibraryEntry>();
            nextUID = 0;
        }

        IList<IPuzzleLibraryEntry> IPuzzleLibrary.Entries
        {
            get
            {
                return Entries.Cast<IPuzzleLibraryEntry>().ToList();
            }
        }

        public IList<PuzzleLibraryEntry> Entries
        {
            get
            {
                return entries.AsReadOnly();
            }
        }

        public PuzzleLibraryEntry GetEntryWithId(int id)
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

        IPuzzleLibraryEntry IPuzzleLibrary.Create(Puzzle puzzle, string author)
        {
            return Create( puzzle, author );
        }

        public PuzzleLibraryEntry Create( Puzzle puzzle, string author)
        {
            var newEntry = new PuzzleLibraryEntry( nextUID++, puzzle, author );

            entries.Add( newEntry );

            return newEntry;
        }

        public void Add(PuzzleLibraryEntry libraryEntry)
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
            return Equals( obj as PuzzleLibrary );
        }

        public bool Equals(PuzzleLibrary library)
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

    internal class PuzzleLibraryEntry : IPuzzleLibraryEntry, IComparable<PuzzleLibraryEntry>
    {
        private readonly int uid;

        private Puzzle puzzle;

        private readonly string author;

        public PuzzleLibraryEntry( int uid, Puzzle puzzle, string author )
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
            return Equals( obj as PuzzleLibraryEntry );
        }

        public bool Equals( PuzzleLibraryEntry that )
        {
            return this.uid == that.uid;
        }

        public override int GetHashCode()
        {
            return uid.GetHashCode();
        }

        public int CompareTo( PuzzleLibraryEntry other )
        {
            return this.uid.CompareTo( other.uid );
        }
    }
}
