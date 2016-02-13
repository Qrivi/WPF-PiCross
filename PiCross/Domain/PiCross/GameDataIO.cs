using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IO;

namespace PiCross
{
    internal class GameDataIO
    {
        public GameData Read(Stream stream)
        {
            using ( var zipArchive = new ZipArchive( stream, ZipArchiveMode.Read, true ) )
            {
                return new Reader( zipArchive ).Read();
            }
        }

        public void Write( GameData gameData, Stream stream )
        {
            using ( var zipArchive = new ZipArchive( stream, ZipArchiveMode.Create, true ) )
            {
                new Writer( zipArchive ).Write(gameData);
            }
        }

        private static string GetLibraryEntryPath( PuzzleLibraryEntry libraryEntry )
        {
            return string.Format( "library/entry{0}.txt", libraryEntry.UID.ToString().PadLeft( 5, '0' ) );
        }

        private static string GetPlayerProfilePath( PlayerProfile playerProfile )
        {
            return string.Format( "players/{0}.txt", playerProfile.Name );
        }

        private static int ExtractEntryID(string filename)
        {
            var regex = new Regex( @"^library/entry(\d+)\.txt$" );
            var match = regex.Match( filename );

            if ( match.Success )
            {
                return int.Parse( match.Groups[1].Value );
            }
            else
            {
                throw new IOException();
            }
        }

        private static string ExtractPlayerName( string filename )
        {
            var regex = new Regex( @"^players/(.*)\.txt$" );
            var match = regex.Match( filename );

            if ( match.Success )
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new IOException();
            }
        }


        private class Reader
        {
            private readonly ZipArchive zipArchive;

            private PuzzleLibrary library;

            private PlayerDatabase playerDatabase;

            public Reader(ZipArchive zipArchive)
            {
                if ( zipArchive == null )
                {
                    throw new ArgumentNullException( "zipArchive" );
                }
                else
                {
                    this.zipArchive = zipArchive;                    
                }
            }

            public GameData Read()
            {
                this.library = PuzzleLibrary.CreateEmpty();
                this.playerDatabase = PlayerDatabase.CreateEmpty();

                var libraryFiles = new List<ZipArchiveEntry>();
                var playerFiles = new List<ZipArchiveEntry>();

                foreach ( var zipEntry in zipArchive.Entries )
                {
                    if ( zipEntry.FullName.StartsWith( "library/" ) )
                    {
                        libraryFiles.Add( zipEntry );
                    }
                    else if ( zipEntry.FullName.StartsWith( "players/" ) )
                    {
                        playerFiles.Add( zipEntry );
                    }
                    else
                    {
                        throw new IOException();
                    }
                }

                foreach ( var libraryFile in libraryFiles)
                {
                    ReadLibraryEntry( libraryFile );
                }

                foreach ( var playerFile in playerFiles )
                {
                    ReadPlayerInformation( playerFile );
                }

                return new GameData( library, playerDatabase );
            }

            private void ReadLibraryEntry(ZipArchiveEntry entry)
            {
                using ( var zipStream = entry.Open() )
                {
                    using ( var zipStreamReader = new StreamReader(zipStream))
                    {
                        var uid = GameDataIO.ExtractEntryID( entry.FullName );
                        var author = zipStreamReader.ReadLine();
                        var puzzle = new PuzzleSerializer().Read( zipStreamReader );

                        library.Add( new PuzzleLibraryEntry( uid, puzzle, author ) );
                    }
                }
            }

            private void ReadPlayerInformation(ZipArchiveEntry entry)
            {
                var playerName = GameDataIO.ExtractPlayerName( entry.FullName );
                var playerProfile = this.playerDatabase.CreateNewProfile( playerName );

                using ( var zipStream = entry.Open() )
                {
                    using ( var zipStreamReader = new StreamReader( zipStream ) )
                    {
                        string line;

                        while ( (line = zipStreamReader.ReadLine()) != null )
                        {
                            var parts = line.Split( ' ' );
                            var uid = int.Parse( parts[0] );
                            var bestTime = double.Parse( parts[1] );
                            var libraryEntry = library.GetEntryWithId( uid );

                            playerProfile.PuzzleInformation[libraryEntry].BestTime.Value = TimeSpan.FromMilliseconds( bestTime );
                        }
                    }
                }
            }
        }

        private class Writer
        {
            private readonly ZipArchive zipArchive;

            public Writer(ZipArchive zipArchive )
            {
                if ( zipArchive == null )
                {
                    throw new ArgumentNullException( "zipArchive" );
                }
                else
                {
                    this.zipArchive = zipArchive;
                }
            }

            public void Write(GameData gameData)
            {
                WriteLibrary(gameData.Library);
                WritePlayerDatabase( gameData.Library, gameData.PlayerDatabase );
            }

            private void WriteLibrary(PuzzleLibrary library)
            {
                foreach ( var libraryEntry in library.Entries )
                {
                    WriteLibraryEntry( libraryEntry );
                }
            }

            private void WriteLibraryEntry( PuzzleLibraryEntry libraryEntry )
            {
                var path = GameDataIO.GetLibraryEntryPath( libraryEntry );
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

            private void WritePlayerDatabase(PuzzleLibrary library, PlayerDatabase playerDatabase)
            {
                foreach ( var playerName in playerDatabase.PlayerNames)
                {
                    var playerProfile = playerDatabase[playerName];
                    var path = GameDataIO.GetPlayerProfilePath( playerProfile );
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
