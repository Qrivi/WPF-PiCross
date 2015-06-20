using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;

namespace PiCross.Game
{
    public class Puzzle
    {
        private readonly ISequence<Constraints> columnConstraints;

        private readonly ISequence<Constraints> rowConstraints;

        private readonly IGrid<Square> grid;

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
                return new Puzzle( columnConstraints: columnConstraints, rowConstraints: rowConstraints, grid: solverGrid.Squares.Copy() );
            }
        }

        public static Puzzle FromGrid( IGrid<Square> grid )
        {
            var editorGrid = new EditorGrid( grid );

            var columnConstraints = editorGrid.DeriveColumnConstraints();
            var rowConstraints = editorGrid.DeriveRowConstraints();

            return new Puzzle( columnConstraints: columnConstraints, rowConstraints: rowConstraints, grid: grid.Copy() );
        }

        public static Puzzle FromRowStrings( params string[] rows )
        {
            return FromGrid( Square.CreateGrid( rows ) );
        }

        private Puzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, IGrid<Square> grid )
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
            else if ( columnConstraints.Length != grid.Width )
            {
                throw new ArgumentException( "columnConstraints and grid do not agree on width" );
            }
            else if ( rowConstraints.Length != grid.Height )
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

        public IGrid<Square> Grid
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

        public int Width
        {
            get
            {
                return this.grid.Width;
            }
        }

        public int Height
        {
            get
            {
                return this.grid.Height;
            }
        }
    }
}
