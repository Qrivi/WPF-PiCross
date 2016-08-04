﻿using DataStructures;
using PiCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    internal class PlayablePuzzle : IPlayablePuzzle
    {
        private readonly PlayGrid playGrid;

        private readonly IGrid<PlayablePuzzleSquare> puzzleSquares;

        private readonly ISequence<PlayablePuzzleConstraints> columnConstraints;

        private readonly ISequence<PlayablePuzzleConstraints> rowConstraints;

        private readonly Cell<bool> isSolved;

        public PlayablePuzzle() { }

        public PlayablePuzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
            : this( new PlayGrid( columnConstraints: columnConstraints, rowConstraints: rowConstraints ) )
        {
            // NOP            
        }

        public PlayablePuzzle( PlayGrid playGrid )
        {
            if ( playGrid == null )
            {
                throw new ArgumentNullException( "playGrid" );
            }
            else
            {
                this.playGrid = playGrid;
                this.puzzleSquares = playGrid.Squares.Map( ( position, var ) => new PlayablePuzzleSquare( this, var, position ) ).Copy();
                this.columnConstraints = this.playGrid.ColumnConstraints.Map( constraints => new PlayablePuzzleConstraints( constraints ) ).Copy();
                this.rowConstraints = this.playGrid.RowConstraints.Map( constraints => new PlayablePuzzleConstraints( constraints ) ).Copy();
                this.isSolved = Cell.Derived( DeriveIsSolved );
            }
        }

        private bool DeriveIsSolved()
        {
            return columnConstraints.Items.All( x => x.IsSatisfied.Value ) && rowConstraints.Items.All( x => x.IsSatisfied.Value );
        }

        public Cell<bool> IsSolved
        {
            get
            {
                return isSolved;
            }
        }
    
        public IGrid<IPlayablePuzzleSquare> Grid
        {
            get
            {
                return this.puzzleSquares;
            }
        }

        public ISequence<IPlayablePuzzleConstraints> ColumnConstraints
        {
            get
            {
                return this.columnConstraints;
            }
        }

        public ISequence<IPlayablePuzzleConstraints> RowConstraints
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
            RefreshIsSolved();
        }

        private void Refresh()
        {
            RefreshSquares();
            RefreshConstraints();
            RefreshIsSolved();
        }

        private void RefreshIsSolved()
        {
            isSolved.Refresh();
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

        private static void RefreshConstraints(PlayablePuzzleConstraints constraints)
        {
            constraints.IsSatisfied.Refresh();

            foreach ( var value in constraints.Values.Items )
            {
                value.IsSatisfied.Refresh();
            }
        }

        private class PlayablePuzzleSquare : IPlayablePuzzleSquare
        {
            private readonly PlayablePuzzleSquareContentsCell contents;

            private readonly Vector2D position;

            public PlayablePuzzleSquare( PlayablePuzzle parent, IVar<Square> contents, Vector2D position )
            {
                this.contents = new PlayablePuzzleSquareContentsCell( parent, contents, position );
                this.position = position;
            }

            Cell<Square> IPlayablePuzzleSquare.Contents
            {
                get
                {
                    return contents;
                }
            }

            public PlayablePuzzleSquareContentsCell Contents
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

        private class PlayablePuzzleSquareContentsCell : ManualCell<Square>
        {
            private readonly PlayablePuzzle parent;

            private readonly IVar<Square> contents;

            private readonly Vector2D position;

            public PlayablePuzzleSquareContentsCell( PlayablePuzzle parent, IVar<Square> contents, Vector2D position )
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

        private class PlayablePuzzleConstraints : IPlayablePuzzleConstraints
        {
            private readonly ISequence<PlayablePuzzleConstraintsValue> constraints;

            private readonly ReadonlyManualCell<bool> isSatisfied;

            public PlayablePuzzleConstraints( PlayGridConstraints constraints )
            {
                this.constraints = constraints.Values.Map( constraint => new PlayablePuzzleConstraintsValue( constraint ) ).Copy();
                this.isSatisfied = new ReadonlyManualCell<bool>( () => constraints.IsSatisfied );
            }

            ISequence<IPlayablePuzzleConstraintsValue> IPlayablePuzzleConstraints.Values
            {
                get
                {
                    return constraints;
                }
            }

            public ISequence<PlayablePuzzleConstraintsValue> Values
            {
                get
                {
                    return constraints;
                }
            }

            Cell<bool> IPlayablePuzzleConstraints.IsSatisfied
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

        private class PlayablePuzzleConstraintsValue : IPlayablePuzzleConstraintsValue
        {
            private readonly ReadonlyManualCell<bool> isSatisfied;

            private readonly PlayGridConstraintValue constraint;

            public PlayablePuzzleConstraintsValue( PlayGridConstraintValue constraint )
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

            Cell<bool> IPlayablePuzzleConstraintsValue.IsSatisfied
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
