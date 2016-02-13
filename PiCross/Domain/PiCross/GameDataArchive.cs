using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiCross.PiCross
{
    internal class GameDataArchive : IDisposable
    {
        private readonly ZipArchive zipArchive;

        public GameDataArchive( ZipArchive zipArchive )
        {
            this.zipArchive = zipArchive;
        }

        private Puzzle ReadPuzzle( StreamReader streamReader )
        {
            return new PuzzleSerializer().Read( streamReader );
        }

        public IList<int> PuzzleLibraryUIDs
        {
            get
            {
                return ( from entry in this.zipArchive.Entries
                         let uid = ExtractEntryID( entry.FullName )
                         where uid.HasValue
                         select uid.Value ).ToList();
            }
        }

        public IList<string> PlayerNames
        {
            get
            {
                return ( from entry in this.zipArchive.Entries
                         let playerName = ExtractPlayerName( entry.FullName )
                         where playerName != null
                         select playerName ).ToList();
            }
        }

        public PuzzleLibraryEntry ReadPuzzleLibraryEntry( int id )
        {
            var path = GetLibraryEntryPath( id );

            using ( var reader = OpenZipArchiveEntryForReading( path ) )
            {
                var author = reader.ReadLine();
                var puzzle = ReadPuzzle( reader );

                return new PuzzleLibraryEntry( id, puzzle, author );
            }
        }

        public InMemoryPlayerProfile ReadPlayerProfile( string playerName )
        {
            var path = GetPlayerProfilePath( playerName );

            using ( var reader = OpenZipArchiveEntryForReading( path ) )
            {
                var playerProfile = new InMemoryPlayerProfile( playerName );
                var entryCount = int.Parse( reader.ReadLine() );

                for ( var i = 0; i != entryCount; ++i )
                {
                    var parts = reader.ReadLine().Split( ' ' );
                    var uid = int.Parse( parts[0] );
                    var bestTime = long.Parse( parts[1] );

                    playerProfile.PuzzleInformation[uid].BestTime.Value = TimeSpan.FromTicks( bestTime );
                }

                return playerProfile;
            }
        }

        public void UpdateLibraryEntry( PuzzleLibraryEntry entry )
        {
            var path = GetLibraryEntryPath( entry.UID );

            using ( var writer = OpenZipArchiveEntryForWriting( path ) )
            {
                writer.WriteLine( entry.Author );
                new PuzzleSerializer().Write( writer, entry.Puzzle );
            }
        }

        public void UpdatePlayerProfile( InMemoryPlayerProfile playerProfile )
        {
            var path = GetPlayerProfilePath( playerProfile.Name );

            using ( var writer = OpenZipArchiveEntryForWriting( path ) )
            {
                foreach ( var id in playerProfile.PuzzleInformation.EntryUIDs )
                {
                    var bestTime = playerProfile.PuzzleInformation[id].BestTime;

                    if ( bestTime.Value.HasValue )
                    {
                        writer.WriteLine( "{0} {1}", id, bestTime.Value.Value.Ticks );
                    }
                }
            }
        }

        private StreamReader OpenZipArchiveEntryForReading( string path )
        {
            return new StreamReader( OpenZipArchive( path ) );
        }

        private StreamWriter OpenZipArchiveEntryForWriting( string path )
        {
            return new StreamWriter( OpenZipArchive( path ) );
        }

        private Stream OpenZipArchive( string path )
        {
            return zipArchive.GetEntry( path ).Open() ?? CreateAndOpenZipArchive( path );
        }

        private Stream CreateAndOpenZipArchive( string path )
        {
            return zipArchive.CreateEntry( path, CompressionLevel.Optimal ).Open();
        }

        private static string GetLibraryEntryPath( int id )
        {
            return string.Format( "library/entry{0}.txt", id.ToString().PadLeft( 5, '0' ) );
        }

        private static string GetPlayerProfilePath( string playerName )
        {
            return string.Format( "players/{0}.txt", playerName );
        }

        private static int? ExtractEntryID( string filename )
        {
            var regex = new Regex( @"^library/entry(\d+)\.txt$" );
            var match = regex.Match( filename );

            if ( match.Success )
            {
                return int.Parse( match.Groups[1].Value );
            }
            else
            {
                return null;
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
                return null;
            }
        }

        public void Dispose()
        {
            this.zipArchive.Dispose();
        }
    }
}
