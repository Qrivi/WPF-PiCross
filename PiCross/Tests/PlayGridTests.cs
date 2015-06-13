using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Game;
using PiCross.DataStructures;
using System.Linq;

namespace PiCross.Tests
{
    [TestClass]
    public class PlayGridTests
    {
        [TestMethod]
        [TestCategory( "PlayGrid" )]
        [Description( "All squares are set to Square.UNKNOWN during initialization" )]
        public void AllSquaresUnknownAfterCreation()
        {
            var playGrid = CreatePlayGrid(
                "x.",
                "x." );

            Assert.IsTrue( playGrid.Squares.Items.All( x => x.Value == Square.UNKNOWN ) );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        [Description( "All constraints must be unsatisfied" )]
        public void AllConstraintsAreUnsatisfiedAfterCreation()
        {
            var playGrid = CreatePlayGrid(
                "x.",
                "x." );

            foreach ( var constraint in playGrid.ColumnConstraints.Items.Concat( playGrid.RowConstraints.Items ) )
            {
                Assert.IsFalse( constraint.IsSatisfied );

                foreach ( var value in constraint.Values.Items )
                {
                    Assert.IsFalse( value.IsSatisfied );
                }
            }
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation1()
        {
            var playGrid = CreatePlayGrid(
                "X ",
                "x." );

            Assert.IsFalse( playGrid.ColumnConstraints[0].IsSatisfied );
            Assert.IsFalse( playGrid.ColumnConstraints[1].IsSatisfied );

            Assert.IsTrue( playGrid.RowConstraints[0].IsSatisfied );
            Assert.IsFalse( playGrid.RowConstraints[1].IsSatisfied );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation2()
        {
            var playGrid = CreatePlayGrid(
                "X XX xxx.x",
                "x........." );

            CheckSatisfaction( playGrid.ColumnConstraints, "ffffffffff" );
            CheckSatisfaction( playGrid.RowConstraints, "ff" );

            CheckSatisfactions( playGrid.RowConstraints, "ttff", "f" );
            CheckSatisfactions( playGrid.ColumnConstraints, "f", "", "f", "f", "", "f", "f", "f", "", "f" );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation3()
        {
            var playGrid = CreatePlayGrid(
                "x. XX",
                "x.. .",
                "...  ",
                "X..X ",
                "X..XX" );

            CheckSatisfaction( playGrid.ColumnConstraints, "ffftf" );
            CheckSatisfaction( playGrid.RowConstraints, "fffff" );

            CheckSatisfactions( playGrid.RowConstraints, "ft", "f", "", "ff", "ff" );
            CheckSatisfactions( playGrid.ColumnConstraints, "ff", "", "", "tt", "ft" );
        }

        private static void CheckSatisfaction( ISequence<PlayGridConstraints> constraints, string expected )
        {
            Assert.AreEqual( CreateBooleans( expected ), constraints.Map( c => c.IsSatisfied ) );
        }

        private static void CheckSatisfactions( ISequence<PlayGridConstraints> constraints, params string[] expecteds )
        {
            Assert.AreEqual( constraints.Length, expecteds.Length );

            foreach ( var i in constraints.Indices )
            {
                var actual = constraints[i];
                var expected = expecteds[i];

                Assert.AreEqual( CreateBooleans( expected ), actual.Values.Map( x => x.IsSatisfied ) );
            }
        }

        private static ISequence<bool> CreateBooleans( string str )
        {
            return Sequence.FromString( str ).Map( c =>
            {
                switch ( c )
                {
                    case 't':
                        return true;

                    case 'f':
                        return false;

                    default:
                        throw new ArgumentException( "Invalid character" );
                }
            } );
        }

        private static PlayGrid CreatePlayGrid( params string[] rows )
        {
            var grid = Grid.CreateCharacterGrid( rows );

            var editorGridData = grid.Map( c =>
                {
                    switch ( c )
                    {
                        case '.':
                        case ' ':
                            return '.';

                        case 'x':
                        case 'X':
                            return 'x';

                        default:
                            throw new ArgumentException( "Invalid character" );
                    }
                } );

            var editorGrid = new EditorGrid( Square.CreateGrid( editorGridData ) );

            var playGridData = Square.CreateGrid( grid.Map( c =>
                {
                    switch ( c )
                    {
                        case 'x':
                        case '.':
                            return '?';

                        case 'X':
                            return 'x';

                        case ' ':
                            return '.';

                        default:
                            throw new ArgumentException( "Invalid character" );
                    }
                } ) );

            var playGrid = editorGrid.CreatePlayGrid();
            playGrid.Squares.Overwrite<Square>( playGridData );

            return playGrid;
        }
    }
}
