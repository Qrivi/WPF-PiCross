using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Utility
{
    interface ITimer
    {
        event Action<int> Tick;

        void Start();
        void Stop();
        void Reset();
    }
}
