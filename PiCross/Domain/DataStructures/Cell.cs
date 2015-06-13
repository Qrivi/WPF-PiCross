using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    internal class Cell<T> : Var<T>, ICell<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialValue">
        /// Cell's initial value.
        /// </param>
        public Cell( T initialValue = default(T) )
            : base( initialValue )
        {
            // NOP
        }

        /// <summary>
        /// Value of the cell.
        /// </summary>
        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if ( !AreEqual( base.Value, value ) )
                {
                    base.Value = value;
                    NotifyObservers();
                }
            }
        }

        protected void NotifyObservers( [CallerMemberName] string propertyName = null )
        {
            if ( PropertyChanged != null )
            {
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
            }
        }

        /// <summary>
        /// Event fired whenever the cell's value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return string.Format( "CELL[{0}]", this.Value != null ? this.Value.ToString() : "null" );
        }
    }
}
