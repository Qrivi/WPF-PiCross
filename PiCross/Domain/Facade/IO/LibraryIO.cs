using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.IO
{
    public class LibraryIO
    {
        public ILibrary Load( Stream stream )
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

            private void WriteEntry( ILibraryEntry entry )
            {
                WritePuzzle( entry.Puzzle );
                WriteAuthor( entry.Author );
            }

            private void WriteAuthor( string author )
            {
                WriteLine( author );
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
                WriteLine( row.Map( x => Square.FromBool( x ).Symbol ).Join() );
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
