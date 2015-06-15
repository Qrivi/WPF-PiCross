using PiCross.DataStructures;
using PiCross.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;

namespace PiCross.Facade.Solving
{
    public class Puzzle : IPuzzle
    {
        private readonly PlayGrid playGrid;

        private readonly IGrid<PuzzleSquare> puzzleSquares;

        private readonly ISequence<PuzzleConstraints> columnConstraints;

        private readonly ISequence<PuzzleConstraints> rowConstraints;

        public Puzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
            : this( new PlayGrid( columnConstraints: columnConstraints, rowConstraints: rowConstraints ) )
        {
            // NOP            
        }

        public Puzzle( PlayGrid playGrid )
        {
            if ( playGrid == null )
            {
                throw new ArgumentNullException( "playGrid" );
            }
            else
            {
                this.playGrid = playGrid;
                this.puzzleSquares = playGrid.Squares.Map( var => new PuzzleSquare( this, var ) ).Copy();
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

        public ISequence<IPuzzleConstraints> ColumnConstraints
        {
            get
            {
                return this.columnConstraints;
            }
        }

        public ISequence<IPuzzleConstraints> RowConstraints
        {
            get
            {
                return this.rowConstraints;
            }
        }

        private void Refresh()
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

        private static void RefreshConstraints( ISequence<PuzzleConstraints> constraints )
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

            public PuzzleSquare( Puzzle parent, IVar<Square> contents )
            {
                this.contents = new PuzzleSquareContentsCell( parent, contents );
            }

            Cell<Square> IPuzzleSquare.Contents
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
            private readonly Puzzle parent;

            private readonly IVar<Square> contents;

            public PuzzleSquareContentsCell( Puzzle parent, IVar<Square> contents )
                : base( contents.Value )
            {
                this.parent = parent;
                this.contents = contents;
            }

            protected override Square ReadValue()
            {
                return this.contents.Value;
            }

            protected override void WriteValue( Square value )
            {
                this.contents.Value = value;

                parent.Refresh();
            }
        }

        private class PuzzleConstraints : IPuzzleConstraints
        {
            private readonly ISequence<PuzzleConstraintsValue> constraints;

            private readonly ReadonlyManualCell<bool> isSatisfied;

            public PuzzleConstraints( PlayGridConstraints constraints )
            {
                this.constraints = constraints.Values.Map( constraint => new PuzzleConstraintsValue( constraint ) ).Copy();
                this.isSatisfied = new ReadonlyManualCell<bool>( () => constraints.IsSatisfied );
            }

            ISequence<IPuzzleConstraintsValue> IPuzzleConstraints.Values
            {
                get
                {
                    return constraints;
                }
            }

            public ISequence<PuzzleConstraintsValue> Constraints
            {
                get
                {
                    return constraints;
                }
            }

            Cell<bool> IPuzzleConstraints.IsSatisfied
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

        private class PuzzleConstraintsValue : IPuzzleConstraintsValue
        {
            private readonly ReadonlyManualCell<bool> isSatisfied;

            private readonly PlayGridConstraintValue constraint;

            public PuzzleConstraintsValue( PlayGridConstraintValue constraint )
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

            Cell<bool> IPuzzleConstraintsValue.IsSatisfied
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
