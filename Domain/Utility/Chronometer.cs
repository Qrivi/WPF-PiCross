using System;
using Cells;

namespace Utility
{
    public class Chronometer : IChronometer
    {
        private DateTime lastTick;
        private bool started;

        public Chronometer()
        {
            started = false;
            TotalTime = Cell.Create(TimeSpan.Zero);
        }

        public void Start()
        {
            started = true;
            lastTick = DateTime.Now;
        }

        public void Pause()
        {
            started = false;
        }

        public void Reset()
        {
            TotalTime.Value = TimeSpan.Zero;
        }

        public void Tick()
        {
            if (started)
            {
                var now = DateTime.Now;
                var delta = now - lastTick;
                lastTick = now;

                TotalTime.Value += delta;
            }
        }

        public Cell<TimeSpan> TotalTime { get; }
    }
}