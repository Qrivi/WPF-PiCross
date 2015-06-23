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
        ObservableCollection<Puzzle> Puzzles { get; }
    }

    public class Library : ILibrary
    {
        private readonly ObservableCollection<Puzzle> puzzles;

        public static Library CreateEmpty()
        {
            return new Library();
        }

        public static Library FromPuzzles( params Puzzle[] puzzles )
        {
            var library = CreateEmpty();

            foreach ( var puzzle in puzzles )
            {
                library.Puzzles.Add( puzzle );
            }

            return library;
        }

        private Library()
        {
            this.puzzles = new ObservableCollection<Puzzle>();
        }

        public ObservableCollection<Puzzle> Puzzles
        {
            get
            {
                return puzzles;
            }
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

                Write( library );

                writer.Flush();
            }

            private void Write( ILibrary library )
            {
                WriteLine( library.Puzzles.Count.ToString() );

                foreach ( var puzzle in library.Puzzles )
                {
                    Write( puzzle );
                }
            }

            private void Write( Puzzle puzzle )
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
                    library.Puzzles.Add( ReadPuzzle() );
                }
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
