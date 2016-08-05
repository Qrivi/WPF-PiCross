using Cells;

namespace DataStructures
{
    public interface ISignal
    {
        void Send();
    }

    public class SignalFactory<T>
    {
        public SignalFactory(Cell<T> cell)
        {
            Cell = cell;
        }

        public SignalFactory(T initialCellContents = default(T))
            : this(Cells.Cell.Create(initialCellContents))
        {
            // NOP
        }

        public Cell<T> Cell { get; }

        public ISignal CreateSignal(T value)
        {
            return new Signal(Cell, value);
        }

        private class Signal : ISignal
        {
            private readonly Cell<T> cell;
            private readonly T value;

            public Signal(Cell<T> cell, T value)
            {
                this.cell = cell;
                this.value = value;
            }

            public void Send()
            {
                cell.Value = value;
            }
        }
    }
}