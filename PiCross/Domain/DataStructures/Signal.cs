using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross.DataStructures
{
    public interface ISignal
    {
        void Send();
    }

    public class SignalFactory<T>
    {
        private readonly Cell<T> cell;

        public SignalFactory( Cell<T> cell )
        {
            this.cell = cell;
        }

        public SignalFactory( T initialCellContents = default(T) )
            : this( Cells.Cell.Create<T>( initialCellContents ) )
        {
            // NOP
        }

        public Cell<T> Cell
        {
            get
            {
                return cell;
            }
        }

        public ISignal CreateSignal( T value )
        {
            return new Signal( cell, value );
        }

        private class Signal : ISignal
        {
            private readonly T value;

            private readonly Cell<T> cell;

            public Signal( Cell<T> cell, T value )
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
