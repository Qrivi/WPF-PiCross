using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public interface ILibrary
    {
        ObservableCollection<ILibraryEntry> Entries { get; }
    }

    public interface ILibraryEntry
    {
        Puzzle Puzzle { get; }

        string Author { get; }
    }

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

        public LibraryEntry(Puzzle puzzle, string author)
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

        public bool Equals(LibraryEntry that)
        {
            return that != null && this.puzzle.Equals( that.puzzle ) && this.author == that.author;
        }

        public override int GetHashCode()
        {
            return puzzle.GetHashCode() ^ author.GetHashCode();
        }
    }

    public class LibraryIO
    {
        public ILibrary Load(Stream stream)
        {
            var loader = new LibraryReader( stream );

            return loader.Result;
        }

        public void Save( ILibrary library, Stream stream )
        {
            new LibraryWriter( library, stream );
        }        

        private class LibraryWriter
        {
            private readonly Stream stream;

            private readonly StreamWriter writer;

            public LibraryWriter( ILibrary library, Stream stream )
            {
                this.stream = stream;
                this.writer = new StreamWriter( stream );

                WriteLibrary( library );

                writer.Flush();
            }

            private void WriteLibrary( ILibrary library )
            {
                WriteLine( library.Entries.Count.ToString() );

                foreach ( var entry in library.Entries )
                {
                    WriteEntry( entry );
                }
            }

            private void WriteEntry(ILibraryEntry entry)
            {               
                WritePuzzle( entry.Puzzle );
                WriteAuthor( entry.Author );
            }

            private void WriteAuthor(string author)
            {
                WriteLine(author);
            }

            private void WritePuzzle( Puzzle puzzle )
            {
                WritePuzzleSize( puzzle.Size );
                WritePuzzleGrid( puzzle.Grid );
            }

            private void WritePuzzleSize( Size size )
            {
                WriteLine( "{0} {1}", size.Width, size.Height );
            }

            private void WritePuzzleGrid( IGrid<bool> grid )
            {
                foreach ( var row in grid.Rows )
                {
                    WritePuzzleGridRow( row );
                }
            }

            private void WritePuzzleGridRow( ISequence<bool> row )
            {
                WriteLine( row.Map( x => Square.FromBool(x).Symbol ).Join() );
            }

            private void WriteLine( string str, params object[] args )
            {
                writer.WriteLine( str, args );
            }
        }

        private class LibraryReader
        {
            private readonly Stream stream;

            private readonly StreamReader reader;

            private readonly Library library;

            public LibraryReader( Stream stream )
            {
                this.library = Library.CreateEmpty();
                this.stream = stream;
                this.reader = new StreamReader( stream );

                Read();
            }

            public ILibrary Result
            {
                get
                {
                    return library;
                }
            }

            private void Read()
            {
                var puzzleCount = int.Parse( ReadLine() );

                for ( var i = 0; i != puzzleCount; ++i )
                {
                    library.Entries.Add( ReadEntry() );
                }
            }

            private ILibraryEntry ReadEntry()
            {
                var puzzle = ReadPuzzle();
                var author = ReadAuthor();

                return new LibraryEntry( puzzle, author );
            }

            private string ReadAuthor()
            {
                return ReadLine();
            }

            private Puzzle ReadPuzzle()
            {
                var size = ReadPuzzleSize();
                var grid = ReadPuzzleGrid( size );

                return Puzzle.FromGrid( grid );
            }

            private Size ReadPuzzleSize()
            {
                var line = ReadLine();
                var ns = line.Split( ' ' ).Select( int.Parse ).ToArray();

                var width = ns[0];
                var height = ns[1];

                return new Size( width, height );
            }

            private IGrid<Square> ReadPuzzleGrid( Size size )
            {
                var rows = ReadLines( size.Height );

                return Grid.Create( size, position => rows[position.Y][position.X] ).Map( Square.FromSymbol );
            }

            private string ReadLine()
            {
                var result = reader.ReadLine();

                return result;
            }

            private string[] ReadLines( int n )
            {
                var result = new string[n];

                for ( var i = 0; i != n; ++i )
                {
                    result[i] = ReadLine();
                }

                return result;
            }
        }
    }
}
