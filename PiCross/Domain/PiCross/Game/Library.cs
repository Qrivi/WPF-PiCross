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
    internal class Library : ILibrary
    {
        private readonly ObservableCollection<ILibraryEntry> entries;

        private int nextUID;

        public static Library CreateEmpty()
        {
            return new Library();
        }

        private Library()
        {
            this.entries = new ObservableCollection<ILibraryEntry>();
            nextUID = 0;
        }

        public ObservableCollection<ILibraryEntry> Entries
        {
            get
            {
                return entries;
            }
        }

        public ILibraryEntry Create( Puzzle puzzle, string author)
        {
            var newEntry = new LibraryEntry( nextUID++, puzzle, author );

            entries.Add( newEntry );

            return newEntry;
        }
    }

    internal class LibraryEntry : ILibraryEntry
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
    }
}
