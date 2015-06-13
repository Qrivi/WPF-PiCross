using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Tests
{
    [TestClass]
    public class Facade_ManualPuzzle : TestBase
    {
        [TestMethod]
        [TestCategory( "ManualPuzzle" )]
        public void ContentsChangeNotification()
        {
            var puzzle = CreateManualPuzzle(
                "."
                );
            var square = puzzle[new Vector2D( 0, 0 )];

            var flag = Flag.Create( square.Contents );

            Assert.IsFalse( flag.Status );
            Assert.AreSame( Square.UNKNOWN, square.Contents.Value );

            square.Contents.Value = Square.EMPTY;

            Assert.IsFalse( flag.Status );
            Assert.AreSame( Square.EMPTY, square.Contents.Value );

            puzzle.Refresh();

            Assert.IsTrue( flag.Status );
            Assert.AreSame( Square.EMPTY, square.Contents.Value );
        }

        [TestMethod]
        [TestCategory( "ManualPuzzle" )]
        public void ConstraintsSatisfactionChangeNotification()
        {
            var puzzle = CreateManualPuzzle(
                "x"
                );
            var square = puzzle[new Vector2D( 0, 0 )];
            var constraint = puzzle.RowConstraints( 0 );

            var flag = Flag.Create( constraint.IsSatisfied );

            Assert.IsFalse( flag.Status );
            Assert.IsFalse( constraint.IsSatisfied.Value );            

            square.Contents.Value = Square.FILLED;

            Assert.IsFalse( flag.Status );
            Assert.IsTrue( constraint.IsSatisfied.Value );

            puzzle.Refresh();

            Assert.IsTrue( flag.Status );
            Assert.IsTrue( constraint.IsSatisfied.Value );
        }

        [TestMethod]
        [TestCategory( "ManualPuzzle" )]
        public void ConstraintValueSatisfactionChangeNotification()
        {
            var puzzle = CreateManualPuzzle(
                "x"
                );
            var square = puzzle[new Vector2D( 0, 0 )];
            var constraints = puzzle.RowConstraints( 0 );
            var value = constraints.Constraints[0];

            var flag = Flag.Create( constraints.Constraints[0].IsSatisfied );

            Assert.IsFalse( flag.Status );
            Assert.IsFalse( value.IsSatisfied.Value );

            square.Contents.Value = Square.FILLED;

            Assert.IsFalse( flag.Status );
            Assert.IsTrue( value.IsSatisfied.Value );

            puzzle.Refresh();

            Assert.IsTrue( flag.Status );
            Assert.IsTrue( value.IsSatisfied.Value );
        }

        private class Flag
        {
            public static Flag<T> Create<T>( ICell<T> cell )
            {
                return new Flag<T>( cell );
            }
        }

        private class Flag<T>
        {
            public Flag( ICell<T> cell )
            {
                cell.PropertyChanged += ( sender, args ) => { Status = true; };
            }

            public bool Status
            {
                get;
                set;
            }
        }
    }
}
