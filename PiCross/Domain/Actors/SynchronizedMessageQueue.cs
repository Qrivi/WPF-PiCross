using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Actors
{
    public class SynchronizedMessageQueue<T> : IMessageQueue<T>
    {
        private readonly IInbox<T> _inbox;

        private readonly IOutbox<T> _outbox;

        public SynchronizedMessageQueue( IMessageQueue<T> messageQueue )
        {
            _inbox = new InboxImplementation( messageQueue );
            _outbox = new OutboxImplementation( messageQueue );
        }

        public IInbox<T> Inbox
        {
            get
            {
                return _inbox;
            }
        }

        public IOutbox<T> Outbox
        {
            get
            {
                return _outbox;
            }
        }

        private class InboxImplementation : IInbox<T>
        {
            private IMessageQueue<T> _synchronizedMessageQueue;

            public InboxImplementation( IMessageQueue<T> synchronizedMessageQueue )
            {
                _synchronizedMessageQueue = synchronizedMessageQueue;
            }

            public bool IsEmpty
            {
                get
                {
                    lock ( _synchronizedMessageQueue )
                    {
                        return _synchronizedMessageQueue.Inbox.IsEmpty;
                    }
                }
            }

            public T Receive()
            {
                lock ( _synchronizedMessageQueue )
                {
                    return _synchronizedMessageQueue.Inbox.Receive();
                }
            }

            public void Empty()
            {
                lock ( _synchronizedMessageQueue )
                {
                    _synchronizedMessageQueue.Inbox.Empty();
                }
            }
        }

        private class OutboxImplementation : IOutbox<T>
        {
            private IMessageQueue<T> _synchronizedMessageQueue;

            public OutboxImplementation( IMessageQueue<T> synchronizedMessageQueue )
            {
                _synchronizedMessageQueue = synchronizedMessageQueue;
            }

            public void Send( T message )
            {
                lock ( _synchronizedMessageQueue )
                {
                    _synchronizedMessageQueue.Outbox.Send( message );
                }
            }
        }
    }
}
