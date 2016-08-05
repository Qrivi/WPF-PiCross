using System;
using System.Linq;
using Cells;
using DataStructures;
using AmbiguityEnum = PiCross.Ambiguity;

namespace PiCross
{
    internal class PuzzleEditor : IPuzzleEditor
    {
        private readonly IGrid<Cell<Ambiguity>> ambiguityGrid;

        private readonly ISequence<PuzzleEditorColumnConstraints> columnConstraints;
        private readonly EditorGrid editorGrid;

        private readonly IGrid<PuzzleEditorSquare> facadeGrid;

        private readonly ISequence<PuzzleEditorRowConstraints> rowConstraints;

        private AmbiguityChecker ambiguityChecker;

        public PuzzleEditor(EditorGrid editorGrid)
        {
            if (editorGrid == null)
            {
                throw new ArgumentNullException("grid");
            }
            this.editorGrid = editorGrid;
            ambiguityChecker = new AmbiguityChecker(editorGrid.DeriveColumnConstraints(),
                editorGrid.DeriveRowConstraints());
            ambiguityGrid = ambiguityChecker.Ambiguities.Map((Ambiguity x) => Cell.Create(x)).Copy();

            facadeGrid =
                editorGrid.Contents.Map(position => new PuzzleEditorSquare(this, position, ambiguityGrid[position]))
                    .Copy();
            columnConstraints =
                editorGrid.Contents.ColumnIndices.Select(x => new PuzzleEditorColumnConstraints(editorGrid, x))
                    .ToSequence();
            rowConstraints =
                editorGrid.Contents.RowIndices.Select(y => new PuzzleEditorRowConstraints(editorGrid, y)).ToSequence();
        }

        public bool IsAmbiguityResolved
        {
            get { return ambiguityChecker.IsAmbiguityResolved; }
        }

        public void ResolveAmbiguity()
        {
            if (!ambiguityChecker.IsAmbiguityResolved)
            {
                ambiguityChecker.Resolve();

                RefreshAmbiguities();
            }
        }

        public IGrid<IPuzzleEditorSquare> Grid
        {
            get { return facadeGrid; }
        }

        public ISequence<IPuzzleEditorConstraints> ColumnConstraints
        {
            get { return columnConstraints; }
        }

        public ISequence<IPuzzleEditorConstraints> RowConstraints
        {
            get { return rowConstraints; }
        }

        public Puzzle BuildPuzzle()
        {
            return editorGrid.ToPuzzle();
        }

        public void ResolveAmbiguityStep()
        {
            if (!ambiguityChecker.IsAmbiguityResolved)
            {
                ambiguityChecker.Step();

                RefreshAmbiguities();
            }
        }

        private void OnSquareChanged(Vector2D position)
        {
            RefreshSquare(position);
            RefreshColumnConstraints(position.X);
            RefreshRowConstraints(position.Y);
            ResetAmbiguities();
        }

        private void ResetAmbiguities()
        {
            ambiguityChecker = new AmbiguityChecker(editorGrid.DeriveColumnConstraints(),
                editorGrid.DeriveRowConstraints());
            RefreshAmbiguities();
        }

        private void RefreshSquare(Vector2D position)
        {
            facadeGrid[position].Refresh();
        }

        private void RefreshColumnConstraints(int x)
        {
            columnConstraints[x].Refresh();
        }

        private void RefreshRowConstraints(int x)
        {
            rowConstraints[x].Refresh();
        }

        private void RefreshAmbiguities()
        {
            ((IGrid<IVar<Ambiguity>>) ambiguityGrid).Overwrite(ambiguityChecker.Ambiguities);
        }

        private class PuzzleEditorSquare : IPuzzleEditorSquare
        {
            public PuzzleEditorSquare(PuzzleEditor parent, Vector2D position, Cell<Ambiguity> ambiguity)
            {
                IsFilled = new PuzzleEditorSquareContentsCell(parent, position);
                Position = position;
                Ambiguity = ambiguity;
            }

            public Cell<bool> IsFilled { get; }

            public Cell<Ambiguity> Ambiguity { get; }

            public Vector2D Position { get; }

            public void Refresh()
            {
                IsFilled.Refresh();
            }
        }

        private class PuzzleEditorSquareContentsCell : ManualCell<bool>
        {
            private readonly IVar<Square> contents;
            private readonly PuzzleEditor parent;

            private readonly Vector2D position;

            public PuzzleEditorSquareContentsCell(PuzzleEditor parent, Vector2D position)
                : base(SquareToBool(parent.editorGrid.Squares[position]))
            {
                this.parent = parent;
                contents = parent.editorGrid.Contents[position];
                this.position = position;
            }

            private static bool SquareToBool(Square square)
            {
                return square == Square.FILLED;
            }

            private static Square BoolToSquare(bool b)
            {
                return b ? Square.FILLED : Square.EMPTY;
            }

            protected override bool ReadValue()
            {
                return SquareToBool(parent.editorGrid.Squares[position]);
            }

            protected override void WriteValue(bool value)
            {
                var square = BoolToSquare(value);

                if (contents.Value != square)
                {
                    contents.Value = square;

                    parent.OnSquareChanged(position);
                }
            }
        }

        private abstract class PuzzleEditorConstraints : IPuzzleEditorConstraints
        {
            protected PuzzleEditorConstraints(Func<Constraints> constraintsFetcher)
            {
                Constraints = Cell.Derived(() => constraintsFetcher());
            }

            public Cell<Constraints> Constraints { get; }

            public void Refresh()
            {
                Constraints.Refresh();
            }
        }

        private class PuzzleEditorRowConstraints : PuzzleEditorConstraints
        {
            public PuzzleEditorRowConstraints(EditorGrid parent, int row)
                : base(() => parent.DeriveRowConstraints(row))
            {
                // NOP
            }
        }

        private class PuzzleEditorColumnConstraints : PuzzleEditorConstraints
        {
            public PuzzleEditorColumnConstraints(EditorGrid parent, int column)
                : base(() => parent.DeriveColumnConstraints(column))
            {
                // NOP
            }
        }
    }
}