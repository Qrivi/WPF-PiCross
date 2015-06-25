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
        private readonly IPuzzleFormat format;

        public LibraryIO()
            : this( new CondensedFormat() )
        {
            // NOP
        }

        public LibraryIO(IPuzzleFormat format)
        {
            this.format = format;
        }

        public ILibrary Load( Stream stream )
        {
            var loader = new LibraryReader( format, stream );

            return loader.Result;
        }

        public void Save( ILibrary library, Stream stream )
        {
            new LibraryWriter( format, library, stream );
        }

        private class LibraryWriter
        {
            private readonly Stream stream;

            private readonly StreamWriter writer;

            private readonly IPuzzleFormat format;

            public LibraryWriter( IPuzzleFormat format, ILibrary library, Stream stream )
            {
                this.format = format;
                this.stream = stream;

                using ( this.writer = new StreamWriter( stream, Encoding.UTF8, 1024, true ) )
                {
                    WriteLibrary( library );
                }
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

            private void WritePuzzle(Puzzle puzzle)
            {
                format.Write( writer, puzzle );   
            }

            private void WriteAuthor( string author )
            {
                WriteLine( author );
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

            private readonly IPuzzleFormat format;

            public LibraryReader( IPuzzleFormat format, Stream stream )
            {
                this.format = format;
                this.library = Library.CreateEmpty();
                this.stream = stream;

                using ( this.reader = new StreamReader( stream, Encoding.UTF8, true, 1024, true ) )
                {
                    Read();
                }
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

            private Puzzle ReadPuzzle()
            {
                return format.Read( reader );
            }

            private string ReadAuthor()
            {
                return ReadLine();
            }

            private string ReadLine()
            {
                var result = reader.ReadLine();

                return result;
            }
        }
    }
}
