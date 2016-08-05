using System;
using System.Linq;
using Cells;
using DataStructures;

namespace PiCross
{
    internal class PlayGrid
    {
        public PlayGrid(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints,
            IGrid<Square> squares)
        {
            if (columnConstraints == null)
            {
                throw new ArgumentNullException("columnConstraints");
            }
            if (rowConstraints == null)
            {
                throw new ArgumentNullException("rowConstraints ");
            }
            if (squares == null)
            {
                throw new ArgumentNullException("squares");
            }
            if (columnConstraints.Length != squares.Size.Width)
            {
                throw new ArgumentException("Number of column constraints should be equal to grid width");
            }
            if (rowConstraints.Length != squares.Size.Height)
            {
                throw new ArgumentException("Number of row constraints should be equal to grid height");
            }
            Squares = squares.Map(sqr => new Var<Square>(sqr)).Copy();

            ColumnConstraints = (from i in Squares.ColumnIndices
                let constraints = columnConstraints[i]
                let slice = new Slice(Squares.Column(i).Map(var => var.Value))
                select new PlayGridConstraints(slice, constraints)).ToSequence();

            RowConstraints = (from i in Squares.RowIndices
                let constraints = rowConstraints[i]
                let slice = new Slice(Squares.Row(i).Map(var => var.Value))
                select new PlayGridConstraints(slice, constraints)).ToSequence();
        }

        public PlayGrid(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints)
            : this(
                columnConstraints, rowConstraints,
                Grid.CreateVirtual(new Size(columnConstraints.Length, rowConstraints.Length), _ => Square.UNKNOWN))
        {
            // NOP
        }

        public IGrid<IVar<Square>> Squares { get; }

        public ISequence<PlayGridConstraints> ColumnConstraints { get; }

        public ISequence<PlayGridConstraints> RowConstraints { get; }
    }

    internal class PlayGridConstraints
    {
        private readonly Constraints constraints;

        private readonly Slice slice;

        public PlayGridConstraints(Slice slice, Constraints constraints)
        {
            this.slice = slice;
            this.constraints = constraints;

            Values = Sequence.FromFunction(constraints.Values.Length,
                i => new PlayGridConstraintValue(slice, constraints, i));
        }

        public ISequence<PlayGridConstraintValue> Values { get; }

        public bool IsSatisfied
        {
            get { return constraints.IsSatisfied(slice); }
        }
    }

    internal class PlayGridConstraintValue
    {
        private readonly Constraints constraints;

        private readonly int index;
        private readonly Slice slice;

        public PlayGridConstraintValue(Slice slice, Constraints constraints, int index)
        {
            this.slice = slice;
            this.constraints = constraints;
            this.index = index;
        }

        public bool IsSatisfied
        {
            get { return !constraints.UnsatisfiedValueRange(slice).Contains(index); }
        }

        public int Value
        {
            get { return constraints.Values[index]; }
        }
    }
}