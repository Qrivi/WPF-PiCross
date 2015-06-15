using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Cells
{
    internal class Derived<T> : CellImplementation<T>
    {
        private readonly Func<T> function;

        public Derived( Func<T> function )
            : base( function() )
        {
            this.function = function;
        }

        public override void Refresh()
        {
            base.Value = function();
        }

        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }
    }

}
