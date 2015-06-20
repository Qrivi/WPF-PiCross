using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;

namespace PiCross.Game
{
    public class SolverGrid
    {
        private readonly IGrid<IVar<Square>> squares;

        private readonly ISequence<Constraints> columnConstraints;

        private readonly ISequence<Constraints> rowConstraints;

        public SolverGrid( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
        {
            if ( columnConstraints == null )
            {
                throw new ArgumentNullException( "columnConstraints" );
            }
            else if ( columnConstraints.Length == 0 )
            {
                throw new ArgumentException( "There must be at least one column" );
            }
            else if ( rowConstraints == null )
            {
                throw new ArgumentNullException( "rowConstraints" );
            }
            else if ( rowConstraints.Length == 0 )
            {
                throw new ArgumentException( "There must be at least one row" );
            }
            else
            {
                this.columnConstraints = columnConstraints;
                this.rowConstraints = rowConstraints;

                var width = columnConstraints.Length;
                var height = rowConstraints.Length;
                this.squares = Grid.Create( new Size( width, height ), p => new Var<Square>( Square.UNKNOWN ) );
            }
        }

        private ISequence<IVar<Square>> Column( int x )
        {
            return this.squares.Column( x );
        }

        private ISequence<IVar<Square>> Row( int y )
        {
            return this.squares.Row( y );
        }

        public Slice ColumnSlice( int x )
        {
            return new Slice( Column( x ).Map( v => v.Value ) );
        }

        public Slice RowSlice( int y )
        {
            return new Slice( Row( y ).Map( v => v.Value ) );
        }

        private void Overwrite( ISequence<IVar<Square>> target, ISequence<Square> source )
        {
            if ( target == null )
            {
                throw new ArgumentNullException( "target" );
            }
            else if ( source == null )
            {
                throw new ArgumentNullException( "source" );
            }
            else if ( target.Length != source.Length )
            {
                throw new ArgumentException( "source and target must have same length" );
            }
            else
            {
                foreach ( var index in target.Indices )
                {
                    target[index].Value = source[index];
                }
            }
        }

        private void OverwriteColumn( int x, Slice slice )
        {
            Overwrite( Column( x ), slice.Squares );
        }

        private void OverwriteRow( int y, Slice slice )
        {
            Overwrite( Row( y ), slice.Squares );
        }

        public void RefineColumn( int x )
        {
            var refined = ColumnSlice( x ).Refine( columnConstraints[x] );

            OverwriteColumn( x, refined );
        }

        public void RefineRow( int y )
        {
            var refined = RowSlice( y ).Refine( rowConstraints[y] );

            OverwriteRow( y, refined );
        }

        public int CountUnknowns()
        {
            return squares.Items.Count( var => var.Value == Square.UNKNOWN );
        }

        public bool IsSolved
        {
            get
            {
                return CountUnknowns() == 0;
            }
        }

        public int Width
        {
            get
            {
                return squares.Size.Width;
            }
        }

        public int Height
        {
            get
            {
                return squares.Size.Height;
            }
        }

        public void RefineColumns()
        {
            for ( var i = 0; i != Width; ++i )
            {
                RefineColumn( i );
            }
        }

        public void RefineRows()
        {
            for ( var i = 0; i != Height; ++i )
            {
                RefineRow( i );
            }
        }


        public void SinglePassRefine()
        {
            RefineColumns();
            RefineRows();
        }

        public void Refine()
        {
            var unknownCount = CountUnknowns();
            var lastUnknownCount = unknownCount + 1;

            while ( unknownCount < lastUnknownCount )
            {
                SinglePassRefine();

                lastUnknownCount = unknownCount;
                unknownCount = CountUnknowns();
            }
        }

        public IGrid<Square> Squares
        {
            get
            {
                return squares.Map( var => var.Value );
            }
        }
    }
}
