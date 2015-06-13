using PiCross.DataStructures;
using PiCross.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Facade.Solving
{
    internal class ManualPuzzle : IPuzzle
    {
        private readonly PlayGrid playGrid;

        private readonly IGrid<PuzzleSquare> puzzleSquares;

        private readonly ISequence<PuzzleConstraints> columnConstraints;

        private readonly ISequence<PuzzleConstraints> rowConstraints;

        public ManualPuzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
        {
            this.playGrid = new PlayGrid( columnConstraints: columnConstraints, rowConstraints: rowConstraints );
            this.puzzleSquares = playGrid.Squares.Map( var => new PuzzleSquare( var ) ).Copy();
            this.columnConstraints = this.playGrid.ColumnConstraints.Map( constraints => new PuzzleConstraints( constraints ) );
            this.rowConstraints = this.playGrid.RowConstraints.Map( constraints => new PuzzleConstraints( constraints ) );
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
            throw new NotImplementedException();
        }

        public IPuzzleConstraints RowConstraints( int x )
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            foreach ( var square in this.puzzleSquares.Items )
            {
                square.Contents.Refresh();
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

            private readonly Derived<bool> isSatisfied;

            public PuzzleConstraints( PlayGridConstraints constraints )
            {
                this.constraints = constraints.Values.Map( constraint => new PuzzleConstraint( constraint ) ).Copy();
                this.isSatisfied = new Derived<bool>( () => constraints.IsSatisfied );
            }

            public ISequence<IPuzzleConstraint> Constraints
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

            public Derived<bool> IsSatisfied
            {
                get
                {
                    return isSatisfied;
                }
            }
        }

        private class PuzzleConstraint : IPuzzleConstraint
        {
            private readonly Derived<bool> isSatisfied;

            private readonly PlayGridConstraintValue constraint;

            public PuzzleConstraint(PlayGridConstraintValue constraint)
            {
                this.constraint = constraint;
                this.isSatisfied = new Derived<bool>( () => constraint.IsSatisfied );
            }

            public int Value
            {
                get
                {
                    return constraint.Value;
                }
            }

            ICell<bool> IPuzzleConstraint.IsSatisfied
            {
                get 
                {
                    return isSatisfied;
                }
            }

            public Derived<bool> IsSatisfied
            {
                get
                {
                    return isSatisfied;
                }
            }
        }
    }
}
