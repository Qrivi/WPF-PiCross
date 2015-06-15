using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Cells
{
    internal class ReadonlyWrapper<T> : Cell<T>
    {
        private readonly Cell<T> wrappedCell;

        public ReadonlyWrapper( Cell<T> wrappedCell )
        {
            if ( wrappedCell == null )
            {
                throw new ArgumentNullException( "wrappedCell" );
            }
            else
            {
                this.wrappedCell = wrappedCell;

                wrappedCell.ValueChanged += NotifyObservers;                    
            }
        }        

        public override T Value
        {
            get
            {
                return wrappedCell.Value;
            }
            set
            {
                throw new InvalidOperationException( "Cell is readonly" );
            }
        }

        public override void Refresh()
        {
            // NOP
        }
    }
}
