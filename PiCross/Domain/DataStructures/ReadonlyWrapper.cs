using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    internal class ReadonlyWrapper<T> : ICell<T>
    {
        private readonly ICell<T> wrappedCell;

        public ReadonlyWrapper( ICell<T> wrappedCell )
        {
            if ( wrappedCell == null )
            {
                throw new ArgumentNullException( "wrappedCell" );
            }
            else
            {
                this.wrappedCell = wrappedCell;
                wrappedCell.PropertyChanged += ( cell, args ) =>
                    {
                        if ( PropertyChanged != null )
                        {
                            PropertyChanged( this, new PropertyChangedEventArgs( "Value" ) );
                        }
                    };
            }
        }        

        public T Value
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
