﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    internal class Library : ILibrary
    {
        private readonly ObservableCollection<ILibraryEntry> entries;

        public static Library CreateEmpty()
        {
            return new Library();
        }

        private Library()
        {
            this.entries = new ObservableCollection<ILibraryEntry>();
        }

        public ObservableCollection<ILibraryEntry> Entries
        {
            get
            {
                return entries;
            }
        }

        public ILibraryEntry Create( Size size, string author )
        {
            var puzzle = Puzzle.CreateEmpty( size );
            var newEntry = new LibraryEntry( puzzle, author );

            entries.Add( newEntry );

            return newEntry;
        }
    }

    internal class LibraryEntry : ILibraryEntry
    {
        private Puzzle puzzle;

        private readonly string author;

        public LibraryEntry( Puzzle puzzle, string author )
        {
            this.puzzle = puzzle;
            this.author = author;
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
            return that != null && this.puzzle.Equals( that.puzzle ) && this.author == that.author;
        }

        public override int GetHashCode()
        {
            return puzzle.GetHashCode() ^ author.GetHashCode();
        }
    }
}
