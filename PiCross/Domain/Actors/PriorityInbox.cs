using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actors
{
    public class PriorityInbox<T> : IInbox<T>
    {
        private readonly List<IInbox<T>> _inboxes;

        public PriorityInbox( params IInbox<T>[] inboxes )
        {
            _inboxes = inboxes.ToList();
        }

        public bool IsEmpty
        {
            get
            {
                return _inboxes.All( inbox => inbox.IsEmpty );
            }
        }

        private IInbox<T> FindFirstNonEmptyInbox()
        {
            return _inboxes.Where( inbox => !inbox.IsEmpty ).First();
        }

        public T Receive()
        {
            return FindFirstNonEmptyInbox().Receive();
        }

        public void Empty()
        {
            throw new InvalidOperationException();
        }
    }
}
