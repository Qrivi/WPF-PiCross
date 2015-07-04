using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross
{
    public class Chronometer
    {
        private DateTime lastTick;

        private bool started;

        private TimeSpan totalTime;

        public Chronometer()
        {
            started = false;
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

        public void Tick()
        {
            if ( started )
            {
                var now = DateTime.Now;
                var delta = now - lastTick;
                lastTick = now;

                totalTime += delta;
            }
        }

        public TimeSpan TotalTime
        {
            get
            {
                return totalTime;
            }
        }
    }
}
