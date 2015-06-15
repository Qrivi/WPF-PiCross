using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Cells
{
    public interface IVar<T>
    {
        T Value { get; set; }
    }
}
