using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiCross.Actors
{
    public class MessageQueue<T> : IMessageQueue<T>
    {
        private readonly IInbox<T> _inbox;

        private readonly IOutbox<T> _outbox;

        public MessageQueue()
        {
            var queue = new Queue<T>();
            _inbox = new InboxImplementation( queue );
            _outbox = new OutboxImplementation( queue );
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
            private readonly Queue<T> _queue;

            public InboxImplementation( Queue<T> queue )
            {
                _queue = queue;
            }

            public bool IsEmpty
            {
                get
                {
                    return _queue.Count == 0;
                }
            }

            public T Receive()
            {
                return _queue.Dequeue();
            }

            public void Empty()
            {
                _queue.Clear();
            }
        }

        private class OutboxImplementation : IOutbox<T>
        {
            private readonly Queue<T> _queue;

            public OutboxImplementation( Queue<T> queue )
            {
                _queue = queue;
            }

            public void Send( T message )
            {
                _queue.Enqueue( message );
            }
        }
    }
}
