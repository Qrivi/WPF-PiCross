using System;
using System.Linq;
using Cells;
using DataStructures;

namespace PiCross
{
    internal class PlayablePuzzle : IPlayablePuzzle
    {
        private readonly ISequence<PlayablePuzzleConstraints> columnConstraints;
        private readonly PlayGrid playGrid;

        private readonly IGrid<PlayablePuzzleSquare> puzzleSquares;

        private readonly ISequence<PlayablePuzzleConstraints> rowConstraints;

        protected Cell<bool> isPlayable;

        protected Cell<bool> isSolved;

        protected Cell<int> mistakes;

        public PlayablePuzzle()
        {
        }

        public PlayablePuzzle(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints)
            : this(new PlayGrid(columnConstraints, rowConstraints))
        {
            // NOP            
        }

        public PlayablePuzzle(PlayGrid playGrid)
        {
            if (playGrid == null)
            {
                throw new ArgumentNullException("playGrid");
            }
            this.playGrid = playGrid;
            puzzleSquares =
                playGrid.Squares.Map((position, var) => new PlayablePuzzleSquare(this, var, position)).Copy();
            columnConstraints =
                this.playGrid.ColumnConstraints.Map(constraints => new PlayablePuzzleConstraints(constraints)).Copy();
            rowConstraints =
                this.playGrid.RowConstraints.Map(constraints => new PlayablePuzzleConstraints(constraints)).Copy();
            isSolved = Cell.Derived(DeriveIsSolved);
            Mistakes = Cell.Create(0);
            isPlayable = Cell.Create(true);
        }

        public Cell<bool> IsSolved
        {
            get { return isSolved; }
        }

        public Cell<bool> IsPlayable
        {
            get { return isPlayable; }
        }

        public Cell<int> Mistakes
        {
            get { return mistakes; }
            set { mistakes = value; }
        }

        public IGrid<IPlayablePuzzleSquare> Grid
        {
            get { return puzzleSquares; }
        }

        public ISequence<IPlayablePuzzleConstraints> ColumnConstraints
        {
            get { return columnConstraints; }
        }

        public ISequence<IPlayablePuzzleConstraints> RowConstraints
        {
            get { return rowConstraints; }
        }

        private bool DeriveIsSolved()
        {
            return columnConstraints.Items.All(x => x.IsSatisfied.Value) &&
                   rowConstraints.Items.All(x => x.IsSatisfied.Value);
        }

        private void Refresh(Vector2D position)
        {
            RefreshSquare(position);
            RefreshColumnConstraints(position.X);
            RefreshRowConstraints(position.Y);
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
            foreach (var square in puzzleSquares.Items)
            {
                square.Contents.Refresh();
            }
        }

        private void RefreshConstraints()
        {
            columnConstraints.Each(RefreshConstraints);
            rowConstraints.Each(RefreshConstraints);
        }

        private void RefreshSquare(Vector2D position)
        {
            puzzleSquares[position].Contents.Refresh();
        }

        private void RefreshColumnConstraints(int x)
        {
            RefreshConstraints(columnConstraints[x]);
        }

        private void RefreshRowConstraints(int y)
        {
            RefreshConstraints(rowConstraints[y]);
        }

        private static void RefreshConstraints(PlayablePuzzleConstraints constraints)
        {
            constraints.IsSatisfied.Refresh();

            foreach (var value in constraints.Values.Items)
            {
                value.IsSatisfied.Refresh();
            }
        }

        private class PlayablePuzzleSquare : IPlayablePuzzleSquare
        {
            public PlayablePuzzleSquare(PlayablePuzzle parent, IVar<Square> contents, Vector2D position)
            {
                Contents = new PlayablePuzzleSquareContentsCell(parent, contents, position);
                Position = position;
            }

            public PlayablePuzzleSquareContentsCell Contents { get; }

            Cell<Square> IPlayablePuzzleSquare.Contents
            {
                get { return Contents; }
            }

            public Vector2D Position { get; }
        }

        private class PlayablePuzzleSquareContentsCell : ManualCell<Square>
        {
            private readonly IVar<Square> contents;
            private readonly PlayablePuzzle parent;

            private readonly Vector2D position;

            public PlayablePuzzleSquareContentsCell(PlayablePuzzle parent, IVar<Square> contents, Vector2D position)
                : base(contents.Value)
            {
                this.parent = parent;
                this.contents = contents;
                this.position = position;
            }

            protected override Square ReadValue()
            {
                return contents.Value;
            }

            protected override void WriteValue(Square value)
            {
                contents.Value = value;

                parent.Refresh(position);
            }
        }

        private class PlayablePuzzleConstraints : IPlayablePuzzleConstraints
        {
            public PlayablePuzzleConstraints(PlayGridConstraints constraints)
            {
                Values = constraints.Values.Map(constraint => new PlayablePuzzleConstraintsValue(constraint)).Copy();
                IsSatisfied = new ReadonlyManualCell<bool>(() => constraints.IsSatisfied);
            }

            public ISequence<PlayablePuzzleConstraintsValue> Values { get; }

            public ReadonlyManualCell<bool> IsSatisfied { get; }

            ISequence<IPlayablePuzzleConstraintsValue> IPlayablePuzzleConstraints.Values
            {
                get { return Values; }
            }

            Cell<bool> IPlayablePuzzleConstraints.IsSatisfied
            {
                get { return IsSatisfied; }
            }
        }

        private class PlayablePuzzleConstraintsValue : IPlayablePuzzleConstraintsValue
        {
            private readonly PlayGridConstraintValue constraint;

            public PlayablePuzzleConstraintsValue(PlayGridConstraintValue constraint)
            {
                this.constraint = constraint;
                IsSatisfied = new ReadonlyManualCell<bool>(() => constraint.IsSatisfied);
            }

            public ReadonlyManualCell<bool> IsSatisfied { get; }

            public int Value
            {
                get { return constraint.Value; }
            }

            Cell<bool> IPlayablePuzzleConstraintsValue.IsSatisfied
            {
                get { return IsSatisfied; }
            }
        }
    }
}