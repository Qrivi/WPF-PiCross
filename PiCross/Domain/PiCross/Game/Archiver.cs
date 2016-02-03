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
            using ( var zipArchive = new ZipArchive( stream, ZipArchiveMode.Create, true ) )
            {
                new Writer( gameData, zipArchive ).Write();
            }
        }

        private static string GetLibraryEntryPath( LibraryEntry libraryEntry )
        {
            return string.Format( "library/entry{0}.txt", libraryEntry.UID.ToString().PadLeft( 5, '0' ) );
        }

        private class Writer
        {
            private readonly ZipArchive zipArchive;

            private readonly GameData gameData;

            public Writer( GameData gameData, ZipArchive zipArchive )
            {
                this.gameData = gameData;
                this.zipArchive = zipArchive;
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
                var path = Archiver.GetLibraryEntryPath( libraryEntry );
                var zipEntry = zipArchive.CreateEntry( path, CompressionLevel.Optimal );

                using ( var zipStream = zipEntry.Open() )
                {
                    using ( var zipStreamWriter = new StreamWriter( zipStream ) )
                    {
                        zipStreamWriter.WriteLine( libraryEntry.Author );
                        WritePuzzle( zipStreamWriter, libraryEntry.Puzzle );
                    }
                }
            }

            private void WritePuzzle(StreamWriter streamWriter, Puzzle puzzle)
            {
                new PuzzleSerializer().Write( streamWriter, puzzle );
            }
        }
    }
}
