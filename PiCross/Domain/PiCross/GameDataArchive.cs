using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiCross
{
    internal interface IGameDataArchive : IDisposable
    {
        IList<int> PuzzleLibraryUIDs { get; }

        IList<string> PlayerNames { get; }

        InMemoryDatabase.InMemoryPuzzleLibraryEntry ReadPuzzleLibraryEntry( int id );

        InMemoryPlayerProfile ReadPlayerProfile( string playerName );

        void UpdateLibraryEntry( InMemoryDatabase.InMemoryPuzzleLibraryEntry entry );

        void UpdatePlayerProfile( InMemoryPlayerProfile playerProfile );
    }

    internal class GameDataArchive : IGameDataArchive
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

        public InMemoryDatabase.InMemoryPuzzleLibraryEntry ReadPuzzleLibraryEntry( int id )
        {
            var path = GetLibraryEntryPath( id );

            using ( var reader = OpenZipArchiveEntryForReading( path ) )
            {
                var author = reader.ReadLine();
                var puzzle = ReadPuzzle( reader );

                return new InMemoryDatabase.InMemoryPuzzleLibraryEntry( id, puzzle, author );
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

                    playerProfile[uid].BestTime = TimeSpan.FromTicks( bestTime );
                }

                return playerProfile;
            }
        }

        public void UpdateLibraryEntry( InMemoryDatabase.InMemoryPuzzleLibraryEntry entry )
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
                var ids = playerProfile.EntryUIDs.ToList();

                writer.WriteLine( ids.Count );
                foreach ( var id in ids )
                {
                    var bestTime = playerProfile[id].BestTime;

                    if ( bestTime.HasValue )
                    {
                        writer.WriteLine( "{0} {1}", id, bestTime.Value.Ticks );
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
            return ( zipArchive.GetEntry( path ) ?? CreateZipArchive( path ) ).Open();
        }

        private ZipArchiveEntry CreateZipArchive( string path )
        {
            return zipArchive.CreateEntry( path, CompressionLevel.Optimal );
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

    internal class AutoCloseGameDataArchive : IGameDataArchive
    {
        private readonly string path;

        public AutoCloseGameDataArchive( string path )
        {
            this.path = path;
        }

        private ZipArchive OpenZipArchiveForReading()
        {
            return new ZipArchive( new FileStream( path, FileMode.Open, FileAccess.Read ), ZipArchiveMode.Read );
        }

        private ZipArchive OpenZipArchiveForWriting()
        {
            return new ZipArchive( new FileStream( path, FileMode.Open, FileAccess.ReadWrite ), ZipArchiveMode.Update );
        }

        private void WithReadOnlyZipArchive( Action<ZipArchive> action )
        {
            using ( var zipArchive = OpenZipArchiveForReading() )
            {
                action( zipArchive );
            }
        }

        private T WithReadOnlyZipArchive<T>( Func<ZipArchive, T> function )
        {
            using ( var zipArchive = OpenZipArchiveForReading() )
            {
                return function( zipArchive );
            }
        }

        private void WithWriteableZipArchive( Action<ZipArchive> action )
        {
            using ( var zipArchive = OpenZipArchiveForWriting() )
            {
                action( zipArchive );
            }
        }

        private T WithWriteableZipArchive<T>( Func<ZipArchive, T> function )
        {
            using ( var zipArchive = OpenZipArchiveForWriting() )
            {
                return function( zipArchive );
            }
        }

        private void WithReadOnlyArchive( Action<GameDataArchive> action )
        {
            WithReadOnlyZipArchive( archive => action( new GameDataArchive( archive ) ) );
        }

        private T WithReadOnlyArchive<T>( Func<GameDataArchive, T> function )
        {
            return WithReadOnlyZipArchive( archive => function( new GameDataArchive( archive ) ) );
        }

        private void WithWriteableArchive( Action<GameDataArchive> action )
        {
            WithWriteableZipArchive( archive => action( new GameDataArchive( archive ) ) );
        }

        private T WithWriteableArchive<T>( Func<GameDataArchive, T> function )
        {
            return WithWriteableZipArchive( archive => function( new GameDataArchive( archive ) ) );
        }

        public IList<int> PuzzleLibraryUIDs
        {
            get
            {
                return WithReadOnlyArchive( archive => archive.PuzzleLibraryUIDs );
            }
        }

        public IList<string> PlayerNames
        {
            get
            {
                return WithReadOnlyArchive( archive => archive.PlayerNames );
            }
        }

        public InMemoryDatabase.InMemoryPuzzleLibraryEntry ReadPuzzleLibraryEntry( int id )
        {
            return WithReadOnlyArchive( archive => archive.ReadPuzzleLibraryEntry( id ) );
        }

        public InMemoryPlayerProfile ReadPlayerProfile( string playerName )
        {
            return WithReadOnlyArchive( archive => archive.ReadPlayerProfile( playerName ) );
        }

        public void UpdateLibraryEntry( InMemoryDatabase.InMemoryPuzzleLibraryEntry entry )
        {
            WithWriteableArchive( archive => archive.UpdateLibraryEntry( entry ) );
        }

        public void UpdatePlayerProfile( InMemoryPlayerProfile playerProfile )
        {
            WithWriteableArchive( archive => archive.UpdatePlayerProfile( playerProfile ) );
        }

        public void Dispose()
        {
            // BOP
        }
    }
}
