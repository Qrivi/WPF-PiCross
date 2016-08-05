using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace Utility
{
    public interface IChronometer
    {
        void Start();
        void Pause();
        void Reset();
        void Tick();

        Cell<TimeSpan> TotalTime { get; }
    }
}
