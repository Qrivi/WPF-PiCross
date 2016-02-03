using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO;

namespace PiCross.Game
{
    public class Archiver
    {
        public void Write( GameData gameData, Stream stream )
        {
            new Writer( gameData, stream ).Write();
        }

        private static string GetLibraryEntryPath(LibraryEntry libraryEntry)
        {
            return string.Format( "library/entry{0}.txt", libraryEntry.UID.ToString().PadLeft( 5, '0' ) );
        }

        private class Writer
        {
            private readonly ZipArchive zipArchive;

            private readonly GameData gameData;

            private readonly ISerializer<LibraryEntry> libraryEntrySerializer;

            public Writer( GameData gameData, Stream stream )
            {
                this.gameData = gameData;
                zipArchive = new ZipArchive( stream, ZipArchiveMode.Create );
                libraryEntrySerializer = new LibraryEntrySerializer( new PuzzleSerializer() );
            }

            public void Write()
            {
                WriteLibrary();
            }

            private void WriteLibrary()
            {
                var library = gameData.Library;

                foreach ( var libraryEntry in library.Entries )
                {
                    // TODO Remove cast
                    WriteLibraryEntry( (LibraryEntry) libraryEntry );
                }
            }

            private void WriteLibraryEntry( LibraryEntry libraryEntry )
            {
                var path = Archiver.GetLibraryEntryPath(libraryEntry);
                var zipEntry = zipArchive.CreateEntry( path, CompressionLevel.Optimal );
                
                using ( var zipStream = zipEntry.Open() )
                {
                    using ( var zipStreamWriter = new StreamWriter(zipStream))
                    {
                        libraryEntrySerializer.Write( zipStreamWriter, libraryEntry );
                    }
                }
            }            
        }
    }
}
