﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public class Library : ILibrary
    {
        private readonly ObservableCollection<ILibraryEntry> puzzles;

        public static Library CreateEmpty()
        {
            return new Library();
        }

        private Library()
        {
            this.puzzles = new ObservableCollection<ILibraryEntry>();
        }

        public ObservableCollection<ILibraryEntry> Entries
        {
            get
            {
                return puzzles;
            }
        }

        public static ILibrary CreateDummyLibrary()
        {
            var library = Library.CreateEmpty();

            {
                var puzzle = Puzzle.FromRowStrings(
                "..x..",
                ".x.x.",
                "x...x",
                ".x.x.",
                "..x.."
                );

                var author = "Woumpousse";

                library.Entries.Add( new LibraryEntry( puzzle, author ) );
            }

            {
                var puzzle = Puzzle.FromRowStrings(
                "x...x",
                ".x.x.",
                "..x..",
                ".x.x.",
                "x...x"
                );

                var author = "Woumpousse";

                library.Entries.Add( new LibraryEntry( puzzle, author ) );
            }

            {
                var puzzle = Puzzle.FromRowStrings(
                ".x..x",
                ".....",
                "..x..",
                "x...x",
                ".xxx."
                );

                var author = "Woumpousse";

                library.Entries.Add( new LibraryEntry( puzzle, author ) );
            }

            {
                var puzzle = Puzzle.FromRowStrings(
                "..x..",
                ".xxx.",
                "xxxxx",
                ".xxx.",
                "..x.."
                );

                var author = "Woumpousse";

                library.Entries.Add( new LibraryEntry( puzzle, author ) );
            }

            return library;
        }
    }

    public class LibraryEntry : ILibraryEntry
    {
        private readonly Puzzle puzzle;

        private readonly string author;

        public LibraryEntry( Puzzle puzzle, string author )
        {
            this.puzzle = puzzle;
            this.author = author;
        }

        public Puzzle Puzzle { get { return puzzle; } }

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
