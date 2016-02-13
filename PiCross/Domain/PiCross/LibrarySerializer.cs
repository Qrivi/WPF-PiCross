using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross;
using IO;

namespace PiCross
{
    internal class LibrarySerializer : ISerializer<InMemoryPuzzleLibrary>
    {
        private readonly ISerializer<InMemoryPuzzleLibraryEntry> libraryEntrySerializer;

        public LibrarySerializer()
        {
            libraryEntrySerializer = new LibraryEntrySerializer(new PuzzleSerializer());
        }

        public void Write( StreamWriter writer, InMemoryPuzzleLibrary obj )
        {
            new Writer( writer, obj, libraryEntrySerializer ).Write();
        }

        public InMemoryPuzzleLibrary Read( StreamReader reader )
        {
            return new Reader( reader, libraryEntrySerializer ).Read();
        }

        private class Reader : ReaderBase
        {
            private readonly ISerializer<InMemoryPuzzleLibraryEntry> libraryEntrySerializer;

            public Reader( StreamReader streamReader, ISerializer<InMemoryPuzzleLibraryEntry> libraryEntrySerializer)
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

            public InMemoryPuzzleLibrary Read()
            {
                var count = ReadInteger();
                var library = InMemoryPuzzleLibrary.CreateEmpty();
                
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
            private readonly ISerializer<InMemoryPuzzleLibraryEntry> libraryEntrySerializer;

            private readonly InMemoryPuzzleLibrary library;

            public Writer(StreamWriter streamWriter, InMemoryPuzzleLibrary library, ISerializer<InMemoryPuzzleLibraryEntry> libraryEntrySerializer)
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
                    libraryEntrySerializer.Write( streamWriter, (InMemoryPuzzleLibraryEntry) libraryEntry );
                }
            }
        }
    }

    internal class LibraryEntrySerializer : ISerializer<InMemoryPuzzleLibraryEntry>
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

        public void Write( StreamWriter streamWriter, InMemoryPuzzleLibraryEntry entry )
        {
            new Writer( streamWriter, entry, puzzleSerializer ).Write();
        }

        public InMemoryPuzzleLibraryEntry Read( StreamReader streamReader )
        {
            return new Reader( streamReader, puzzleSerializer ).Read();
        }

        private class Writer : WriterBase
        {
            private readonly InMemoryPuzzleLibraryEntry libraryEntry;

            private readonly ISerializer<Puzzle> puzzleSerializer;

            internal Writer( StreamWriter streamWriter, InMemoryPuzzleLibraryEntry libraryEntry, ISerializer<Puzzle> puzzleSerializer )
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

            internal InMemoryPuzzleLibraryEntry Read()
            {
                var uid = ReadInteger();
                var author = streamReader.ReadLine();
                var puzzle = puzzleSerializer.Read( streamReader );

                return new InMemoryPuzzleLibraryEntry( uid, puzzle, author );
            }
        }
    }
}
