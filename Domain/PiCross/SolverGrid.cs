using System;
using System.Linq;
using Cells;
using DataStructures;

namespace PiCross
{
    internal class SolverGrid
    {
        private readonly ISequence<Constraints> columnConstraints;

        private readonly ISequence<Constraints> rowConstraints;
        private readonly IGrid<IVar<Square>> squares;

        public SolverGrid(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints)
        {
            if (columnConstraints == null)
            {
                throw new ArgumentNullException("columnConstraints");
            }
            if (columnConstraints.Length == 0)
            {
                throw new ArgumentException("There must be at least one column");
            }
            if (rowConstraints == null)
            {
                throw new ArgumentNullException("rowConstraints");
            }
            if (rowConstraints.Length == 0)
            {
                throw new ArgumentException("There must be at least one row");
            }
            this.columnConstraints = columnConstraints;
            this.rowConstraints = rowConstraints;

            var width = columnConstraints.Length;
            var height = rowConstraints.Length;
            squares = Grid.Create(new Size(width, height), p => new Var<Square>(Square.UNKNOWN));
        }

        public bool IsSolved
        {
            get { return CountUnknowns() == 0; }
        }

        public int Width
        {
            get { return squares.Size.Width; }
        }

        public int Height
        {
            get { return squares.Size.Height; }
        }

        public IGrid<Square> Squares
        {
            get { return squares.Map(var => var.Value); }
        }

        public static SolverGrid FromPuzzle(Puzzle puzzle)
        {
            return new SolverGrid(puzzle.ColumnConstraints, puzzle.RowConstraints);
        }

        private ISequence<IVar<Square>> Column(int x)
        {
            return squares.Column(x);
        }

        private ISequence<IVar<Square>> Row(int y)
        {
            return squares.Row(y);
        }

        public Slice ColumnSlice(int x)
        {
            return new Slice(Column(x).Map(v => v.Value));
        }

        public Slice RowSlice(int y)
        {
            return new Slice(Row(y).Map(v => v.Value));
        }

        private bool OverwriteColumn(int x, Slice slice)
        {
            return Column(x).Overwrite(slice.Squares);
        }

        private bool OverwriteRow(int y, Slice slice)
        {
            return Row(y).Overwrite(slice.Squares);
        }

        public bool RefineColumn(int x)
        {
            var refined = ColumnSlice(x).Refine(columnConstraints[x]);

            return OverwriteColumn(x, refined);
        }

        public bool RefineRow(int y)
        {
            var refined = RowSlice(y).Refine(rowConstraints[y]);

            return OverwriteRow(y, refined);
        }

        public int CountUnknowns()
        {
            return squares.Items.Count(var => var.Value == Square.UNKNOWN);
        }

        public bool RefineColumns()
        {
            var changeDetected = false;

            for (var i = 0; i != Width; ++i)
            {
                changeDetected = RefineColumn(i) || changeDetected;
            }

            return changeDetected;
        }

        public bool RefineRows()
        {
            var changeDetected = false;

            for (var i = 0; i != Height; ++i)
            {
                changeDetected = RefineRow(i) || changeDetected;
            }

            return changeDetected;
        }

        public bool SinglePassRefine()
        {
            var columnChanged = RefineColumns();
            var rowChanged = RefineRows();

            return columnChanged || rowChanged;
        }

        public void Refine()
        {
            while (SinglePassRefine()) ;
        }
    }
}