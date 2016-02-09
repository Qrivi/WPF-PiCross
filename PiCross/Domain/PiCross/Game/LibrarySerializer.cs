using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Facade;
using PiCross.Game;
using IO;

namespace PiCross.Game
{
    internal class LibrarySerializer : ISerializer<Library>
    {
        private readonly ISerializer<LibraryEntry> libraryEntrySerializer;

        public LibrarySerializer()
        {
            libraryEntrySerializer = new LibraryEntrySerializer(new PuzzleSerializer());
        }

        public void Write( StreamWriter writer, Library obj )
        {
            new Writer( writer, obj, libraryEntrySerializer ).Write();
        }

        public Library Read( StreamReader reader )
        {
            return new Reader( reader, libraryEntrySerializer ).Read();
        }

        private class Reader : ReaderBase
        {
            private readonly ISerializer<LibraryEntry> libraryEntrySerializer;

            public Reader( StreamReader streamReader, ISerializer<LibraryEntry> libraryEntrySerializer)
                : base( streamReader )
            {
                if ( libraryEntrySerializer == null )
                {
                    throw new ArgumentNullException( "libraryEntrySerializer" );
                }
                else
                {
                    this.libraryEntrySerializer = libraryEntrySerializer;
                }
            }

            public Library Read()
            {
                var count = ReadInteger();
                var library = Library.CreateEmpty();
                
                for ( var i = 0; i != count; ++i )
                {
                    var libraryEntry = libraryEntrySerializer.Read( streamReader );
                    library.Entries.Add( libraryEntry );
                }

                return library;
            }
        }

        private class Writer : WriterBase
        {
            private readonly ISerializer<LibraryEntry> libraryEntrySerializer;

            private readonly Library library;

            public Writer(StreamWriter streamWriter, Library library, ISerializer<LibraryEntry> libraryEntrySerializer)
                :base(streamWriter)
            {
                if ( library == null )
                {
                    throw new ArgumentNullException( "library" );
                }
                else if ( libraryEntrySerializer == null )
                {
                    throw new ArgumentNullException( "libraryEntrySerializer" );
                }
                else
                {
                    this.library = library;
                    this.libraryEntrySerializer = libraryEntrySerializer;
                }
            }

            public void Write()
            {
                streamWriter.WriteLine( library.Entries.Count );

                foreach ( var libraryEntry in library.Entries )
                {
                    libraryEntrySerializer.Write( streamWriter, (LibraryEntry) libraryEntry );
                }
            }
        }
    }

    internal class LibraryEntrySerializer : ISerializer<LibraryEntry>
    {
        private readonly ISerializer<Puzzle> puzzleSerializer;

        internal LibraryEntrySerializer(ISerializer<Puzzle> puzzleSerializer)
        {
            if ( puzzleSerializer == null )
            {
                throw new ArgumentNullException( "puzzleSerializer" );
            }
            else
            {
                this.puzzleSerializer = puzzleSerializer;
            }
        }

        public void Write( StreamWriter streamWriter, LibraryEntry entry )
        {
            new Writer( streamWriter, entry, puzzleSerializer ).Write();
        }

        public LibraryEntry Read( StreamReader streamReader )
        {
            return new Reader( streamReader, puzzleSerializer ).Read();
        }

        private class Writer : WriterBase
        {
            private readonly LibraryEntry libraryEntry;

            private readonly ISerializer<Puzzle> puzzleSerializer;

            internal Writer( StreamWriter streamWriter, LibraryEntry libraryEntry, ISerializer<Puzzle> puzzleSerializer )
                : base( streamWriter )
            {
                if ( libraryEntry == null )
                {
                    throw new ArgumentNullException( "libraryEntry" );
                }
                else if ( puzzleSerializer == null )
                {
                    throw new ArgumentNullException( "puzzleSerializer" );
                }
                else
                {
                    this.libraryEntry = libraryEntry;
                    this.puzzleSerializer = puzzleSerializer;
                }
            }

            internal void Write()
            {
                streamWriter.WriteLine( libraryEntry.UID );
                streamWriter.WriteLine( libraryEntry.Author );
                puzzleSerializer.Write( streamWriter, libraryEntry.Puzzle );
            }
        }

        private class Reader : ReaderBase
        {
            private readonly ISerializer<Puzzle> puzzleSerializer;

            internal Reader(StreamReader streamReader, ISerializer<Puzzle> puzzleSerializer) : base(streamReader)
            {
                if ( puzzleSerializer == null )
                {
                    throw new ArgumentNullException( "puzzleSerializer" );
                }
                else
                {
                    this.puzzleSerializer = puzzleSerializer;
                }
            }

            internal LibraryEntry Read()
            {
                var uid = ReadInteger();
                var author = streamReader.ReadLine();
                var puzzle = puzzleSerializer.Read( streamReader );

                return new LibraryEntry( uid, puzzle, author );
            }
        }
    }
}
