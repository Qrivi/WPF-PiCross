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
                // this.puzzleSquares = playGrid.Squares.Map( var => new PuzzleSquare( this, var ) ).Copy();
                this.puzzleSquares = playGrid.Squares.Map( ( position, var ) => new PuzzleSquare( this, var, position ) ).Copy();
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

        private void Refresh( Vector2D position )
        {
            RefreshSquare( position );
            RefreshColumnConstraints( position.X );
            RefreshRowConstraints( position.Y );
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
            columnConstraints.Each( RefreshConstraints );
            rowConstraints.Each( RefreshConstraints );
        }

        private void RefreshSquare(Vector2D position)
        {
            this.puzzleSquares[position].Contents.Refresh();
        }

        private void RefreshColumnConstraints(int x)
        {
            RefreshConstraints( this.columnConstraints[x] );
        }

        private void RefreshRowConstraints( int y )
        {
            RefreshConstraints( this.rowConstraints[y] );
        }

        private static void RefreshConstraints(PuzzleConstraints constraints)
        {
            constraints.IsSatisfied.Refresh();

            foreach ( var value in constraints.Constraints.Items )
            {
                value.IsSatisfied.Refresh();
            }
        }

        private class PuzzleSquare : IPuzzleSquare
        {
            private readonly PuzzleSquareContentsCell contents;

            private readonly Vector2D position;

            public PuzzleSquare( Puzzle parent, IVar<Square> contents, Vector2D position )
            {
                this.contents = new PuzzleSquareContentsCell( parent, contents, position );
                this.position = position;
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

            public Vector2D Position
            {
                get
                {
                    return position;
                }
            }
        }

        private class PuzzleSquareContentsCell : ManualCell<Square>
        {
            private readonly Puzzle parent;

            private readonly IVar<Square> contents;

            private readonly Vector2D position;

            public PuzzleSquareContentsCell( Puzzle parent, IVar<Square> contents, Vector2D position )
                : base( contents.Value )
            {
                this.parent = parent;
                this.contents = contents;
                this.position = position;
            }

            protected override Square ReadValue()
            {
                return this.contents.Value;
            }

            protected override void WriteValue( Square value )
            {
                this.contents.Value = value;

                parent.Refresh( position );
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
