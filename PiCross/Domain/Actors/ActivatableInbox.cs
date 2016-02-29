using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    public class ActivatableInbox<T> : IInbox<T>
    {
        private readonly IInbox<T> _inbox;

        private bool _active;

        public ActivatableInbox( IInbox<T> inbox, bool active = true )
        {
            _inbox = inbox;
            _active = active;
        }

        public bool IsEmpty
        {
            get
            {
                if ( _active )
                {
                    return _inbox.IsEmpty;
                }
                else
                {
                    return true;
                }
            }
        }

        public T Receive()
        {
            if ( _active )
            {
                return _inbox.Receive();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public void Activate()
        {
            _active = true;
        }

        public void Deactivate()
        {
            _active = false;
        }

        public void Empty()
        {
            _inbox.Empty();
        }
    }
}
