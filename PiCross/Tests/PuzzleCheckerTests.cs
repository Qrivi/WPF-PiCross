using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Cells;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class PuzzleCheckerTests : TestBase
    {
        [TestMethod]
        [TestCategory("PuzzleChecker")]        
        public void FindAmbiguities1()
        {
            var actual = FindAmbiguities( "." );
            var expected = CreateBooleansGrid( "f" );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "PuzzleChecker" )]
        public void FindAmbiguities2()
        {
            var actual = FindAmbiguities( "x" );
            var expected = CreateBooleansGrid( "f" );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "PuzzleChecker" )]
        public void FindAmbiguities3()
        {
            var actual = FindAmbiguities(
                "x.",
                ".x"
                );
            var expected = CreateBooleansGrid( 
                "tt",
                "tt"
                );

            Assert.AreEqual( expected.Size, actual.Size );

            foreach ( var position in actual.AllPositions )
            {
                Assert.AreEqual( expected[position], actual[position], string.Format("Failed at position {0}", position.ToString() ) );
            }
        }

        private IGrid<bool> FindAmbiguities(params string[] rows)
        {
            var checker = CreatePuzzleChecker();
            var puzzle = CreatePuzzle( rows );
            var future = new FutureCell<IGrid<bool>>();

            checker.FindAmbiguities( columnConstraints: puzzle.ColumnConstraints, rowConstraints: puzzle.RowContraints, output: future );
            var result = future.Value;

            checker.Kill();

            return result;
        }
    }
}
