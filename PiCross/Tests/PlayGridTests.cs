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
        }

        private static void CheckSatisfaction( ISequence<PlayGridConstraints> constraints, string expected )
        {
            Assert.AreEqual( CreateBooleans( expected ), constraints.Map( c => c.IsSatisfied ) );
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
