using System.Collections.Generic;
using System.Linq;
using Cells;
using DataStructures;

namespace PiCross
{
    internal class EditorGrid
    {
        private EditorGrid(IGrid<IVar<Square>> grid)
        {
            Contents = grid;
        }

        public EditorGrid(IGrid<Square> grid)
            : this(grid.Map(x => new Var<Square>(x)).Copy())
        {
            // NOP
        }

        private EditorGrid(Size size)
            : this(Grid.Create(size, _ => new Var<Square>(Square.UNKNOWN)))
        {
            // NOP
        }

        public IGrid<IVar<Square>> Contents { get; }

        public IGrid<Square> Squares
        {
            get { return Contents.Map(var => var.Value); }
        }

        public IEnumerable<Slice> Columns
        {
            get { return Contents.ColumnIndices.Select(Column); }
        }

        public IEnumerable<Slice> Rows
        {
            get { return Contents.RowIndices.Select(Row); }
        }

        public Size Size
        {
            get { return Contents.Size; }
        }

        public static EditorGrid FromSize(Size size)
        {
            return new EditorGrid(size);
        }

        public static EditorGrid FromStrings(params string[] rows)
        {
            return new EditorGrid(Square.CreateGrid(rows));
        }

        public static EditorGrid FromPuzzle(Puzzle puzzle)
        {
            return new EditorGrid(puzzle.Grid.Map(b => b ? Square.FILLED : Square.EMPTY));
        }

        public Slice Column(int x)
        {
            return new Slice(Squares.Column(x));
        }

        public Slice Row(int y)
        {
            return new Slice(Squares.Row(y));
        }

        public Constraints DeriveColumnConstraints(int column)
        {
            return Column(column).DeriveConstraints();
        }

        public Constraints DeriveRowConstraints(int row)
        {
            return Row(row).DeriveConstraints();
        }

        public ISequence<Constraints> DeriveColumnConstraints()
        {
            return Sequence.FromEnumerable(Columns.Select(column => column.DeriveConstraints()));
        }

        public ISequence<Constraints> DeriveRowConstraints()
        {
            return Sequence.FromEnumerable(Rows.Select(row => row.DeriveConstraints()));
        }

        public PlayGrid CreatePlayGrid()
        {
            return new PlayGrid(DeriveColumnConstraints(), DeriveRowConstraints());
        }

        public SolverGrid CreateSolverGrid()
        {
            return new SolverGrid(DeriveColumnConstraints(), DeriveRowConstraints());
        }

        public Puzzle ToPuzzle()
        {
            return Puzzle.FromGrid(Contents.Map(cell => cell.Value == Square.FILLED));
        }
    }
}