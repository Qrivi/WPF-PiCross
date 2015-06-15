using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    public interface IVar<T>
    {
        T Value { get; set; }
    }
}
