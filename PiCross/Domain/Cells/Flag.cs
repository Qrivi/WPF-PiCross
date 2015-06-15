using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Cells
{
    public class Flag
    {
        public static Flag<T> Create<T>( Cell<T> cell )
        {
            return new Flag<T>( cell );
        }
    }

    public class Flag<T>
    {
        public Flag( Cell<T> cell )
        {
            cell.ValueChanged += () => { Status = true; };
        }

        public bool Status
        {
            get;
            set;
        }
    }
}
