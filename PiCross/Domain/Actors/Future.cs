using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Actors
{
    public class Future<T>
    {
        private T _value;

        private volatile bool _bound;

        public Future()
        {
            _value = default( T );
            _bound = false;
        }

        public T Value
        {
            get
            {
                lock ( this )
                {
                    if ( !_bound )
                    {
                        Monitor.Wait( this );
                    }

                    return _value;
                }
            }
            set
            {
                lock ( this )
                {
                    if ( _bound )
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        _value = value;
                        _bound = true;

                        Monitor.PulseAll( this );
                    }
                }
            }
        }
    }
}
