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
                new Writer( zipArchive ).Write(gameData);
            }
        }

        private static string GetLibraryEntryPath( LibraryEntry libraryEntry )
        {
            return string.Format( "library/entry{0}.txt", libraryEntry.UID.ToString().PadLeft( 5, '0' ) );
        }

        private static string GetPlayerProfilePath( PlayerProfile playerProfile )
        {
            return string.Format( "players/{0}.txt", playerProfile.Name );
        }

        private class Writer
        {
            private readonly ZipArchive zipArchive;

            public Writer(ZipArchive zipArchive )
            {
                this.zipArchive = zipArchive;
            }

            public void Write(GameData gameData)
            {
                WriteLibrary(gameData.Library);
                WritePlayerDatabase( gameData.Library, gameData.PlayerDatabase );
            }

            private void WriteLibrary(Library library)
            {
                foreach ( var libraryEntry in library.Entries )
                {
                    WriteLibraryEntry( libraryEntry );
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

            private void WritePlayerDatabase(Library library, PlayerDatabase playerDatabase)
            {
                foreach ( var playerName in playerDatabase.PlayerNames)
                {
                    var playerProfile = playerDatabase[playerName];
                    var path = Archiver.GetPlayerProfilePath( playerProfile );
                    var zipEntry = zipArchive.CreateEntry(path, CompressionLevel.Optimal);
                    
                    using ( var zipStream = zipEntry.Open() )
                    {
                        using ( var zipStreamWriter = new StreamWriter(zipStream) )
                        {
                            foreach ( var libraryEntry in library.Entries )
                            {
                                int uid = libraryEntry.UID;
                                var puzzleInformation = playerProfile.PuzzleInformation[libraryEntry];
                                
                                if ( puzzleInformation.BestTime.Value.HasValue )
                                {
                                    var bestTime = puzzleInformation.BestTime.Value.Value;

                                    zipStreamWriter.WriteLine("{0} {1}", uid, bestTime.TotalMilliseconds);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
