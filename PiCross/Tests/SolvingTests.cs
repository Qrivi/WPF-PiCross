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
        public void Test1()
        {
            Check( "x" );
        }

        [TestMethod]
        public void Test2()
        {
            Check( "." );
        }

        [TestMethod]
        public void Test3()
        {
            Check( ".." );
        }

        [TestMethod]
        public void Test4()
        {
            Check( ".x" );
        }

        [TestMethod]
        public void Test5()
        {
            Check( "x." );
        }

        [TestMethod]
        public void Test6()
        {
            Check( 
                "..",
                ".." );
        }

        [TestMethod]
        public void Test7()
        {
            Check(
                "x.",
                "x." );
        }

        [TestMethod]
        public void Test8()
        {
            Check(
                "..",
                "x." );
        }

        [TestMethod]
        public void Test9()
        {
            Check(
                "xx",
                "x." );
        }

        [TestMethod]
        public void Test10()
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
        public void Test11()
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
        public void Test12()
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
            var columnConstraints = editorGrid.DeriveColumnConstraints();
            var rowConstraints = editorGrid.DeriveRowConstraints();
            var solverGrid = new SolverGrid( columnConstraints: columnConstraints, rowConstraints: rowConstraints );

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
