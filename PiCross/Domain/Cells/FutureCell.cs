using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiCross.Cells
{
    public class FutureCell<T> : Cell<T>
    {
        private volatile bool isBound;

        public FutureCell( T initialValue = default(T) )
            : base( initialValue )
        {

        }

        public override T Value
        {
            get
            {
                lock ( this )
                {
                    WaitForBinding();

                    return base.Value;
                }
            }
            set
            {
                lock ( this )
                {
                    if ( isBound )
                    {
                        throw new InvalidOperationException( "Future already bound" );
                    }
                    else
                    {
                        base.Value = value;

                        isBound = true;
                        Monitor.PulseAll( this );
                    }
                }
            }
        }

        private void WaitForBinding()
        {
            if ( !isBound )
            {
                Monitor.Wait( this );
            }
        }

        public override void Refresh()
        {
            throw new InvalidOperationException( "Cannot refresh futures" );
        }
    }
}
