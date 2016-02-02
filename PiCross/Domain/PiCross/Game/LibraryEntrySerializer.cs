using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Facade.IO;
using PiCross.Game;
using IO;

namespace PiCross.PiCross.Facade.IO
{
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
