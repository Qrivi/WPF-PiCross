using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;
using PiCross.DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class SolvingTests
    {
        [TestMethod]
        [TestCategory("SolverGrid")]
        public void Solve1()
        {
            Check( "x" );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve2()
        {
            Check( "." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve3()
        {
            Check( ".." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve4()
        {
            Check( ".x" );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve5()
        {
            Check( "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve6()
        {
            Check( 
                "..",
                ".." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve7()
        {
            Check(
                "x.",
                "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve8()
        {
            Check(
                "..",
                "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve9()
        {
            Check(
                "xx",
                "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve10()
        {
            Check(
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                ".........." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve11()
        {
            Check(
                "..........",
                ".xxxxxxxx.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".xxxxxxxx.",
                ".........." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve12()
        {
            Check(
                ".xxx..xx..",
                "..xxx.....",
                "....xxxx..",
                "......xx..",
                "..xxxxx...",
                ".x.x.xxx.x",
                ".xxx...xxx",
                "...xxxxx..",
                "......xxx.",
                ".........." );
        }


        private static void Check(params string[] rows)
        {
            var editorGrid = EditorGrid.FromStrings( rows );
            var solverGrid = editorGrid.CreateSolverGrid();

            solverGrid.Refine();

            Assert.AreEqual( editorGrid.Squares.Width, solverGrid.Squares.Width );
            Assert.AreEqual( editorGrid.Squares.Height, solverGrid.Squares.Height );

            foreach ( var position in solverGrid.Squares.AllPositions )
            {
                var expected = editorGrid.Squares[position];
                var actual = solverGrid.Squares[position];

                Assert.AreEqual( expected, actual, "Unequal squares at {0}", position );
            }
        }
    }
}
