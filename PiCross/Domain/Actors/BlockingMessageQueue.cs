using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PiCross.Actors
{
    public class BlockingMessageQueue<T> : IMessageQueue<T>
    {
        private readonly IInbox<T> _inbox;

        private readonly IOutbox<T> _outbox;

        public BlockingMessageQueue( IMessageQueue<T> messageQueue )
        {
            var waitHandle = new AutoResetEvent( !messageQueue.Inbox.IsEmpty );

            _inbox = new InboxImplementation( messageQueue.Inbox, waitHandle );
            _outbox = new OutboxImplementation( messageQueue.Outbox, waitHandle );
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
            private readonly IInbox<T> _inbox;

            private readonly WaitHandle _waitHandle;

            public InboxImplementation( IInbox<T> inbox, WaitHandle waitHandle )
            {
                _inbox = inbox;
                _waitHandle = waitHandle;
            }

            public bool IsEmpty
            {
                get
                {
                    return false;
                }
            }

            public T Receive()
            {
                while ( _inbox.IsEmpty )
                {
                    _waitHandle.WaitOne();
                }

                return _inbox.Receive();
            }

            public void Empty()
            {
                _inbox.Empty();
            }
        }

        private class OutboxImplementation : IOutbox<T>
        {
            private readonly IOutbox<T> _outbox;

            private readonly EventWaitHandle _waitHandle;

            public OutboxImplementation( IOutbox<T> outbox, EventWaitHandle waitHandle )
            {
                _outbox = outbox;
                _waitHandle = waitHandle;
            }

            public void Send( T message )
            {
                _outbox.Send( message );
                _waitHandle.Set();
            }
        }
    }
}
