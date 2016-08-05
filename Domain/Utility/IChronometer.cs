using System;
using Cells;

namespace Utility
{
    public interface IChronometer
    {
        Cell<TimeSpan> TotalTime { get; }
        void Start();
        void Pause();
        void Reset();
        void Tick();
    }
}