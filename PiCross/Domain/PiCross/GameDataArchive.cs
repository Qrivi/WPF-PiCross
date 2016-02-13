﻿using System;
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

        public PuzzleLibraryEntry ReadPuzzleLibraryEntry( int id )
        {
            var path = GetLibraryEntryPath( id );

            using ( var reader = OpenZipArchiveEntry( path ) )
            {
                var author = reader.ReadLine();
                var puzzle = ReadPuzzle( reader );

                return new PuzzleLibraryEntry( id, puzzle, author );
            }
        }

        public PlayerProfile ReadPlayerProfile( string playerName )
        {
            var path = GetPlayerProfilePath( playerName );

            using ( var reader = OpenZipArchiveEntry( path ) )
            {
                var playerProfile = new PlayerProfile( playerName );
                var entryCount = int.Parse( reader.ReadLine() );

                for ( var i = 0; i != entryCount; ++i )
                {
                    var parts = reader.ReadLine().Split( ' ' );
                    var uid = int.Parse( parts[0] );
                    var bestTime = double.Parse( parts[1] );

                    // playerProfile.PuzzleInformation[libraryEntry].BestTime.Value = TimeSpan.FromMilliseconds( bestTime );
                }

                return playerProfile;
            }
        }

        private StreamReader OpenZipArchiveEntry( string path )
        {
            return new StreamReader( zipArchive.GetEntry( path ).Open() );
        }

        private static string GetLibraryEntryPath( int id )
        {
            return string.Format( "library/entry{0}.txt", id.ToString().PadLeft( 5, '0' ) );
        }

        private static string GetPlayerProfilePath( string playerName )
        {
            return string.Format( "players/{0}.txt", playerName );
        }

        private static int ExtractEntryID( string filename )
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

        public void Dispose()
        {
            this.zipArchive.Dispose();
        }
    }
}
