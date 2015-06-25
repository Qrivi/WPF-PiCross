using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class PuzzleUIDTests : TestBase
    {
        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack1()
        {
            var puzzle = CreatePuzzle(
                "."
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack2()
        {
            var puzzle = CreatePuzzle(
                "x"
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack3()
        {
            var puzzle = CreatePuzzle(
                ".."
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack4()
        {
            var puzzle = CreatePuzzle(
                ".x"
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack5()
        {
            var puzzle = CreatePuzzle(
                "x."
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack6()
        {
            var puzzle = CreatePuzzle(
                "xx"
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack7()
        {
            var puzzle = CreatePuzzle(
                "........x"
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack8()
        {
            var puzzle = CreatePuzzle(
                "........x.x.x.x.x....x.x.x.x..x..x...x........xxx"
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack9()
        {
            var puzzle = CreatePuzzle(
                ".",
                "."
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack10()
        {
            var puzzle = CreatePuzzle(
                "x",
                "."
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ConvertToUidAndBack11()
        {
            var puzzle = CreatePuzzle(
                ".x..x.x.xx.x.x.x..x.x.xxxx",
                ".xxx.x.x..xx.x.x.xx.x..x..",
                "xx....x.x.x.x.x.xx...x...x",
                "xxx.x..x....x.x.xx.x.xxx.x",
                "xx.x..x.xx..x...x.x.x...x."
                );

            var uid = CreateUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ToAndFromBase64_1()
        {
            var puzzle = CreatePuzzle(
                "." );

            var uid1 = CreateUID( puzzle );
            var b64 = uid1.Base64;
            var uid2 = PuzzleUID.FromBase64( b64 );

            Assert.AreEqual( uid1, uid2 );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ToAndFromBase64_2()
        {
            var puzzle = CreatePuzzle(
                "x" );

            var uid1 = CreateUID( puzzle );
            var b64 = uid1.Base64;
            var uid2 = PuzzleUID.FromBase64( b64 );

            Assert.AreEqual( uid1, uid2 );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ToAndFromBase64_3()
        {
            var puzzle = CreatePuzzle(
                ".." );

            var uid1 = CreateUID( puzzle );
            var b64 = uid1.Base64;
            var uid2 = PuzzleUID.FromBase64( b64 );

            Assert.AreEqual( uid1, uid2 );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ToAndFromBase64_4()
        {
            var puzzle = CreatePuzzle(
                "xx" );

            var uid1 = CreateUID( puzzle );
            var b64 = uid1.Base64;
            var uid2 = PuzzleUID.FromBase64( b64 );

            Assert.AreEqual( uid1, uid2 );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ToAndFromBase64_5()
        {
            var puzzle = CreatePuzzle(
                "...",
                "..." );

            var uid1 = CreateUID( puzzle );
            var b64 = uid1.Base64;
            var uid2 = PuzzleUID.FromBase64( b64 );

            Assert.AreEqual( uid1, uid2 );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void ToAndFromBase64_6()
        {
            var puzzle = CreatePuzzle(
                ".x..x.x.xx.x.x.x..x.x.xxxx",
                ".xxx.x.x..xx.x.x.xx.x..x..",
                "xx....x.x.x.x.x.xx...x...x",
                "xxx.x..x....x.x.xx.x.xxx.x",
                "xx.x..x.xx..x...x.x.x...x."
                );

            var uid1 = CreateUID( puzzle );
            var b64 = uid1.Base64;
            var uid2 = PuzzleUID.FromBase64( b64 );

            Assert.AreEqual( uid1, uid2 );
        }

        public static PuzzleUID CreateUID( Puzzle puzzle )
        {
            return PuzzleUID.FromPuzzle( puzzle );
        }
    }
}
