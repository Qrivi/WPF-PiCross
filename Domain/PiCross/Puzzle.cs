using System;
using System.Linq;
using DataStructures;

namespace PiCross
{
    public class AmbiguousConstraintsException : PiCrossException
    {
        public AmbiguousConstraintsException()
            : base("Ambiguous constraints")
        {
            // NOP
        }
    }

    /// <summary>
    ///     A Puzzle object contains all information about a PiCross puzzle,
    ///     i.e. it contains the row and column constraints as well as the actual solution.
    /// </summary>
    public sealed class Puzzle
    {
        private Puzzle(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, IGrid<bool> grid)
        {
            if (columnConstraints == null)
            {
                throw new ArgumentNullException("columnConstraints");
            }
            if (rowConstraints == null)
            {
                throw new ArgumentNullException("rowConstraints");
            }
            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }
            if (columnConstraints.Length != grid.Size.Width)
            {
                throw new ArgumentException("columnConstraints and grid do not agree on width");
            }
            if (rowConstraints.Length != grid.Size.Height)
            {
                throw new ArgumentException("rowConstraints and grid do not agree on height");
            }
            ColumnConstraints = columnConstraints;
            RowConstraints = rowConstraints;
            Grid = grid;
        }

        /// <summary>
        ///     Grid representing the solution of the puzzle.
        /// </summary>
        public IGrid<bool> Grid { get; }

        /// <summary>
        ///     Row constraints.
        /// </summary>
        public ISequence<Constraints> RowConstraints { get; }

        /// <summary>
        ///     Column constraints.
        /// </summary>
        public ISequence<Constraints> ColumnConstraints { get; }

        /// <summary>
        ///     Size of the puzzle.
        /// </summary>
        public Size Size
        {
            get { return Grid.Size; }
        }

        /// <summary>
        ///     Checks whether the puzzle is solveable based on the constraints.
        /// </summary>
        public bool IsSolvable
        {
            get
            {
                var solverGrid = new SolverGrid(ColumnConstraints, RowConstraints);

                solverGrid.Refine();

                return solverGrid.IsSolved;
            }
        }

        /// <summary>
        ///     Creates a Puzzle from the constraints. Since a Puzzle
        ///     contains the solution, this method solves the given puzzle.
        /// </summary>
        /// <param name="columnConstraints">Column constraints.</param>
        /// <param name="rowConstraints">Row constraints.</param>
        /// <returns>A Puzzle with the given constraints.</returns>
        /// <exception cref="AmbiguousConstraintsException">
        ///     Thrown when the constraints
        ///     don't lead to a single solution.
        /// </exception>
        public static Puzzle FromConstraints(ISequence<Constraints> columnConstraints,
            ISequence<Constraints> rowConstraints)
        {
            var solverGrid = new SolverGrid(columnConstraints, rowConstraints);
            solverGrid.Refine();

            if (!solverGrid.IsSolved)
            {
                throw new ArgumentException("Ambiguous constraints");
            }
            var grid = ConvertSquareGridToBoolGrid(solverGrid.Squares);

            return new Puzzle(columnConstraints, rowConstraints, grid);
        }

        /// <summary>
        ///     Creates a Puzzle from a solution. The constraints
        ///     will be inferred.
        /// </summary>
        /// <param name="grid">Solution represented by a grid of Squares.</param>
        /// <returns>A Puzzle with the given solution.</returns>
        public static Puzzle FromGrid(IGrid<Square> grid)
        {
            var editorGrid = new EditorGrid(grid);

            var columnConstraints = editorGrid.DeriveColumnConstraints();
            var rowConstraints = editorGrid.DeriveRowConstraints();

            var boolGrid = ConvertSquareGridToBoolGrid(grid);

            return new Puzzle(columnConstraints, rowConstraints, boolGrid);
        }

        /// <summary>
        ///     Creates a Puzzle from a solution. The constraints
        ///     will be inferred.
        /// </summary>
        /// <param name="grid">Solution represented by a grid of bools.</param>
        /// <returns>A Puzzle with the given solution.</returns>
        public static Puzzle FromGrid(IGrid<bool> grid)
        {
            return FromGrid(grid.Map(Square.FromBool));
        }

        /// <summary>
        ///     Creates a Puzzle from a sequence of strings representing
        ///     the rows of the solution of a puzzle. A 'x' represents
        ///     a filled cell, a '.' corresponds to an empty cell.
        /// </summary>
        /// <param name="rows">Strings representing rows.</param>
        /// <returns>Puzzle.</returns>
        public static Puzzle FromRowStrings(params string[] rows)
        {
            return FromGrid(Square.CreateGrid(rows));
        }

        /// <summary>
        ///     Creates an empty puzzle with the given size.
        ///     The constraints are all zero, the solution is the empty grid.
        /// </summary>
        /// <param name="size">Size of the puzzle.</param>
        /// <returns>Empty puzzle.</returns>
        public static Puzzle CreateEmpty(Size size)
        {
            return FromGrid(DataStructures.Grid.Create(size, false));
        }

        private static IGrid<bool> ConvertSquareGridToBoolGrid(IGrid<Square> squares)
        {
            return squares.Map(x => (bool) x).Copy();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Puzzle);
        }

        public bool Equals(Puzzle that)
        {
            return that != null && ColumnConstraints.Equals(that.ColumnConstraints) &&
                   RowConstraints.Equals(that.RowConstraints) && Grid.Equals(that.Grid);
        }

        public override int GetHashCode()
        {
            return Size.GetHashCode() ^ ColumnConstraints.GetHashCode() ^ RowConstraints.GetHashCode();
        }

        public override string ToString()
        {
            var rowStrings = from row in Grid.Rows
                select row.Map(x => Square.FromBool(x).Symbol).Join();

            return rowStrings.ToSequence().Join("\n");
        }
    }
}