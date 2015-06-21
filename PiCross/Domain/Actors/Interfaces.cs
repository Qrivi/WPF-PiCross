using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Actors
{
    public interface IInbox<out T>
    {
        bool IsEmpty { get; }

        T Receive();

        void Empty();
    }

    public interface IOutbox<in T>
    {
        void Send( T message );
    }

    public interface IMessageQueue<T>
    {
        IInbox<T> Inbox { get; }

        IOutbox<T> Outbox { get; }
    }
}
