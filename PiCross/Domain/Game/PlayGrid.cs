using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Game
{
    public class PlayGrid
    {
        private readonly IGrid<IVar<Square>> grid;

        private readonly ISequence<Constraints> columnConstraints;

        private readonly ISequence<Constraints> rowConstraints;

        public PlayGrid( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
        {
            this.columnConstraints = columnConstraints;
            this.rowConstraints = rowConstraints;

            var width = columnConstraints.Length;
            var height = rowConstraints.Length;
            this.grid = Grid.Create( width, height, _ => new Var<Square>( Square.UNKNOWN ) );
        }

        public IGrid<IVar<Square>> Squares
        {
            get
            {
                return grid;
            }
        }

        public ISequence<Constraints> ColumnConstraints
        {
            get
            {
                return columnConstraints;
            }
        }

        public ISequence<Constraints> RowConstraints
        {
            get
            {
                return rowConstraints;
            }
        }
    }

    public class PlayGridConstraints
    {
        private readonly ISequence<PlayGridConstraintValue> values;

        public PlayGridConstraints( Slice slice, Constraints constraints )
        {
            this.values = Sequence.FromFunction( constraints.Values.Length, i => new PlayGridConstraintValue( slice, constraints, i ) );
        }

        public ISequence<PlayGridConstraintValue> Values
        {
            get
            {
                return values;
            }
        }
    }

    public class PlayGridConstraintValue
    {
        private readonly Slice slice;

        private readonly Constraints constraints;

        private readonly int index;

        public PlayGridConstraintValue( Slice slice, Constraints constraints, int index )
        {
            this.slice = slice;
            this.constraints = constraints;
            this.index = index;
        }

        public bool IsSatisfied
        {
            get
            {
                return index < constraints.SatisfiedPrefixLength( slice ) || index >= constraints.Values.Length - constraints.SatisfiedSuffixLength( slice );
            }
        }
    }
}
