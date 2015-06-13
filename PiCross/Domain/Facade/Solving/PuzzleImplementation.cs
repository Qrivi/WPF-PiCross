using PiCross.DataStructures;
using PiCross.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Facade.Solving
{
    // TODO Rename it to something Wrapper
    public class ManualPuzzle : IPuzzle
    {
        private readonly PlayGrid playGrid;

        private readonly IGrid<PuzzleSquare> puzzleSquares;

        private readonly ISequence<PuzzleConstraints> columnConstraints;

        private readonly ISequence<PuzzleConstraints> rowConstraints;

        public ManualPuzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
            : this( new PlayGrid( columnConstraints: columnConstraints, rowConstraints: rowConstraints ) )
        {
            // NOP            
        }

        public ManualPuzzle( PlayGrid playGrid )
        {
            if ( playGrid == null )
            {
                throw new ArgumentNullException( "playGrid" );
            }
            else
            {
                this.playGrid = playGrid;
                this.puzzleSquares = playGrid.Squares.Map( var => new PuzzleSquare( var ) ).Copy();
                this.columnConstraints = this.playGrid.ColumnConstraints.Map( constraints => new PuzzleConstraints( constraints ) ).Copy();
                this.rowConstraints = this.playGrid.RowConstraints.Map( constraints => new PuzzleConstraints( constraints ) ).Copy();
            }
        }

        public int Width
        {
            get
            {
                return playGrid.Squares.Width;
            }
        }

        public int Height
        {
            get
            {
                return playGrid.Squares.Height;
            }
        }

        public IPuzzleSquare this[DataStructures.Vector2D position]
        {
            get
            {
                return puzzleSquares[position];
            }
        }

        public IPuzzleConstraints ColumnConstraints( int x )
        {
            return this.rowConstraints[x];
        }

        public IPuzzleConstraints RowConstraints( int y )
        {
            return this.columnConstraints[y];
        }

        public void Refresh()
        {
            RefreshSquares();
            RefreshConstraints();
        }

        private void RefreshSquares()
        {
            foreach ( var square in this.puzzleSquares.Items )
            {
                square.Contents.Refresh();
            }
        }

        private void RefreshConstraints()
        {
            RefreshConstraints( columnConstraints );
            RefreshConstraints( rowConstraints );
        }

        private static void RefreshConstraints(ISequence<PuzzleConstraints> constraints)
        {
            foreach ( var constraint in constraints.Items )
            {
                constraint.IsSatisfied.Refresh();

                foreach ( var value in constraint.Constraints.Items )
                {
                    value.IsSatisfied.Refresh();
                }
            }
        }

        private class PuzzleSquare : IPuzzleSquare
        {
            private readonly PuzzleSquareContentsCell contents;

            public PuzzleSquare( IVar<Square> contents )
            {
                this.contents = new PuzzleSquareContentsCell( contents );
            }

            ICell<Square> IPuzzleSquare.Contents
            {
                get
                {
                    return contents;
                }
            }

            public PuzzleSquareContentsCell Contents
            {
                get
                {
                    return contents;
                }
            }
        }

        private class PuzzleSquareContentsCell : ManualCell<Square>
        {
            private readonly IVar<Square> contents;

            public PuzzleSquareContentsCell( IVar<Square> contents )
                : base( contents.Value )
            {
                this.contents = contents;
            }

            protected override Square ReadValue()
            {
                return this.contents.Value;
            }

            protected override void WriteValue( Square value )
            {
                this.contents.Value = value;
            }
        }

        private class PuzzleConstraints : IPuzzleConstraints
        {
            private readonly ISequence<PuzzleConstraint> constraints;

            private readonly ReadonlyManualCell<bool> isSatisfied;

            public PuzzleConstraints( PlayGridConstraints constraints )
            {
                this.constraints = constraints.Values.Map( constraint => new PuzzleConstraint( constraint ) ).Copy();
                this.isSatisfied = new ReadonlyManualCell<bool>( () => constraints.IsSatisfied );
            }

            ISequence<IPuzzleConstraintsValue> IPuzzleConstraints.Values
            {
                get
                {
                    return constraints;
                }
            }

            public ISequence<PuzzleConstraint> Constraints
            {
                get
                {
                    return constraints;
                }
            }

            ICell<bool> IPuzzleConstraints.IsSatisfied
            {
                get
                {
                    return isSatisfied;
                }
            }

            public ReadonlyManualCell<bool> IsSatisfied
            {
                get
                {
                    return isSatisfied;
                }
            }
        }

        private class PuzzleConstraint : IPuzzleConstraintsValue
        {
            private readonly ReadonlyManualCell<bool> isSatisfied;

            private readonly PlayGridConstraintValue constraint;

            public PuzzleConstraint( PlayGridConstraintValue constraint )
            {
                this.constraint = constraint;
                this.isSatisfied = new ReadonlyManualCell<bool>( () => constraint.IsSatisfied );
            }

            public int Value
            {
                get
                {
                    return constraint.Value;
                }
            }

            ICell<bool> IPuzzleConstraintsValue.IsSatisfied
            {
                get
                {
                    return isSatisfied;
                }
            }

            public ReadonlyManualCell<bool> IsSatisfied
            {
                get
                {
                    return isSatisfied;
                }
            }
        }
    }
}
