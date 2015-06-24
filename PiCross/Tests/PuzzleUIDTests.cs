using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class PuzzleUIDTests : TestBase
    {
        [TestMethod]
        [TestCategory("PuzzleUID")]
        public void Test1()
        {
            var puzzle = CreatePuzzle(
                "."
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test2()
        {
            var puzzle = CreatePuzzle(
                "x"
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test3()
        {
            var puzzle = CreatePuzzle(
                ".."
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test4()
        {
            var puzzle = CreatePuzzle(
                ".x"
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test5()
        {
            var puzzle = CreatePuzzle(
                "x."
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test6()
        {
            var puzzle = CreatePuzzle(
                "xx"
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test7()
        {
            var puzzle = CreatePuzzle(
                "........x"
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test8()
        {
            var puzzle = CreatePuzzle(
                "........x.x.x.x.x....x.x.x.x..x..x...x........xxx"
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test9()
        {
            var puzzle = CreatePuzzle(
                ".",
                "."
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test10()
        {
            var puzzle = CreatePuzzle(
                "x",
                "."
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }

        [TestMethod]
        [TestCategory( "PuzzleUID" )]
        public void Test11()
        {
            var puzzle = CreatePuzzle(
                ".x..x.x.xx.x.x.x..x.x.xxxx",
                ".xxx.x.x..xx.x.x.xx.x..x..",
                "xx....x.x.x.x.x.xx...x...x",
                "xxx.x..x....x.x.xx.x.xxx.x",
                "xx.x..x.xx..x...x.x.x...x."
                );

            var uid = new PuzzleUID( puzzle );

            Assert.AreEqual( puzzle, uid.CreatePuzzle() );
        }
    }
}
