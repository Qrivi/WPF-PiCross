using System;
using System.Collections.Generic;
using System.IO;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;
using Utility;

namespace PiCross.Tests
{
    [TestClass]
    public class ArchiverTests
    {
        [TestMethod]
        public void LoadingSavedGameData1()
        {
            var library = PuzzleLibrary.CreateEmpty();
            var players = new PlayerDatabase();

            var gameData = new GameData(library, players);
            var gameData2 = SaveThenLoad( gameData );

            AssertEqual( gameData, gameData2 );
        }

        [TestMethod]
        public void LoadingSavedGameData2()
        {
            var library = PuzzleLibrary.CreateEmpty();
            var players = new PlayerDatabase();

            library.Create( Puzzle1, "x" );

            var gameData = new GameData( library, players );
            var gameData2 = SaveThenLoad( gameData );

            AssertEqual( gameData, gameData2 );
        }

        [TestMethod]
        public void LoadingSavedGameData3()
        {
            var library = PuzzleLibrary.CreateEmpty();
            var players = new PlayerDatabase();

            library.Create( Puzzle1, "x" );
            library.Create( Puzzle2, "y" );
            library.Create( Puzzle3, "z" );

            var gameData = new GameData( library, players );
            var gameData2 = SaveThenLoad( gameData );

            AssertEqual( gameData, gameData2 );
        }

        [TestMethod]
        public void LoadingSavedGameData4()
        {
            var library = PuzzleLibrary.CreateEmpty();
            var players = new PlayerDatabase();

            var e1 = library.Create( Puzzle1, "x" );
            var e2 = library.Create( Puzzle2, "y" );
            var e3 = library.Create( Puzzle3, "z" );

            var dieter = players.CreateNewProfile( "dieter" );
            dieter.PuzzleInformation[e1].BestTime.Value = TimeSpan.FromMilliseconds( 10000 );

            var gameData = new GameData( library, players );
            var gameData2 = SaveThenLoad( gameData );

            AssertEqual( gameData, gameData2 );
        }

        private Puzzle Puzzle1
        {
            get
            {
                return Puzzle.FromRowStrings( "." );
            }
        }

        private Puzzle Puzzle2
        {
            get
            {
                return Puzzle.FromRowStrings( ".....", "....." );
            }
        }

        private Puzzle Puzzle3
        {
            get
            {
                return Puzzle.FromRowStrings( ".....", "xxxxx" );
            }
        }
        
        private GameData SaveThenLoad(GameData gameData)
        {
            using ( var stream = new MemoryStream() )
            {
                var archiver = new GameDataIO();

                archiver.Write( gameData, stream );
                stream.Seek( 0, SeekOrigin.Begin );
                return archiver.Read( stream );
            }
        }

        private void AssertEqual(GameData x, GameData y)
        {
            AssertEqual( x.Library, y.Library );
            AssertEqual( x.Library, x.PlayerDatabase, y.PlayerDatabase );
        }

        private void AssertEqual(PuzzleLibrary x, PuzzleLibrary y)
        {
            ContainsSameEntriesAs( x, y );
            ContainsSameEntriesAs( y, x );
        }

        private void ContainsSameEntriesAs(PuzzleLibrary x, PuzzleLibrary y)
        {
            foreach ( var xEntry in x.Entries )
            {
                var yEntry = y.GetEntryWithId( xEntry.UID );

                Assert.AreEqual( xEntry.Puzzle, yEntry.Puzzle );
            }
        }

        private void AssertEqual(PuzzleLibrary library, PlayerDatabase x, PlayerDatabase y)
        {
            Assert.AreEqual( x, y );
        }
    }
}
