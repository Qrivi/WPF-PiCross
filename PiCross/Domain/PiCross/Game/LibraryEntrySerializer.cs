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
    internal class LibraryEntrySerializer : ISerializer<ILibraryEntry>
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

        public void Write( StreamWriter streamWriter, ILibraryEntry entry )
        {
            new Writer( streamWriter, entry, puzzleSerializer ).Write();
        }

        public ILibraryEntry Read( StreamReader streamReader )
        {
            return new Reader( streamReader, puzzleSerializer ).Read();
        }

        private class Writer : WriterBase
        {
            private readonly ILibraryEntry libraryEntry;

            private readonly ISerializer<Puzzle> puzzleSerializer;

            internal Writer( StreamWriter streamWriter, ILibraryEntry libraryEntry, ISerializer<Puzzle> puzzleSerializer )
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

            internal ILibraryEntry Read()
            {
                var author = streamReader.ReadLine();
                var puzzle = puzzleSerializer.Read( streamReader );

                return new LibraryEntry( puzzle, author );
            }
        }
    }
}
