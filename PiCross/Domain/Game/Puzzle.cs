using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;

namespace PiCross.Game
{
    public sealed class Puzzle
    {
        private readonly ISequence<Constraints> columnConstraints;

        private readonly ISequence<Constraints> rowConstraints;

        private readonly IGrid<bool> grid;

        public static Puzzle FromConstraints( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
        {
            var solverGrid = new SolverGrid( columnConstraints, rowConstraints );
            solverGrid.Refine();

            if ( !solverGrid.IsSolved )
            {
                throw new ArgumentException( "Ambiguous constraints" );
            }
            else
            {
                var grid = ConvertSquareGridToBoolGrid( solverGrid.Squares );

                return new Puzzle( columnConstraints: columnConstraints, rowConstraints: rowConstraints, grid: grid );
            }
        }

        public static Puzzle FromGrid( IGrid<Square> grid )
        {
            var editorGrid = new EditorGrid( grid );

            var columnConstraints = editorGrid.DeriveColumnConstraints();
            var rowConstraints = editorGrid.DeriveRowConstraints();

            var boolGrid = ConvertSquareGridToBoolGrid( grid );

            return new Puzzle( columnConstraints: columnConstraints, rowConstraints: rowConstraints, grid: boolGrid );
        }

        public static Puzzle FromGrid( IGrid<bool> grid )
        {
            return FromGrid( grid.Map( Square.FromBool ) );
        }

        public static Puzzle FromRowStrings( params string[] rows )
        {
            return FromGrid( Square.CreateGrid( rows ) );
        }

        private static IGrid<bool> ConvertSquareGridToBoolGrid(IGrid<Square> squares)
        {
            return squares.Map( x => (bool) x ).Copy();
        }

        private Puzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, IGrid<bool> grid )
        {
            if ( columnConstraints == null )
            {
                throw new ArgumentNullException( "columnConstraints" );
            }
            else if ( rowConstraints == null )
            {
                throw new ArgumentNullException( "rowContraints" );
            }
            else if ( grid == null )
            {
                throw new ArgumentNullException( "grid" );
            }
            else if ( columnConstraints.Length != grid.Size.Width )
            {
                throw new ArgumentException( "columnConstraints and grid do not agree on width" );
            }
            else if ( rowConstraints.Length != grid.Size.Height )
            {
                throw new ArgumentException( "rowConstraints and grid do not agree on height" );
            }
            else
            {
                this.columnConstraints = columnConstraints;
                this.rowConstraints = rowConstraints;
                this.grid = grid;
            }
        }

        public IGrid<bool> Grid
        {
            get
            {
                return grid;
            }
        }

        public ISequence<Constraints> RowContraints
        {
            get
            {
                return rowConstraints;
            }
        }

        public ISequence<Constraints> ColumnConstraints
        {
            get
            {
                return columnConstraints;
            }
        }

        public Size Size
        {
            get
            {
                return this.grid.Size;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Puzzle );
        }

        public bool Equals(Puzzle that)
        {
            return that != null && this.columnConstraints.Equals( that.columnConstraints ) && this.rowConstraints.Equals( that.rowConstraints ) && this.grid.Equals( that.grid );
        }

        public override int GetHashCode()
        {
            return Size.GetHashCode() ^ columnConstraints.GetHashCode() ^ rowConstraints.GetHashCode();
        }

        public override string ToString()
        {
            var rowStrings = from row in this.Grid.Rows
                             select row.Map( x => Square.FromBool(x).Symbol ).Join();

            return rowStrings.ToSequence().Join( "\n" );
        }
    }
}
