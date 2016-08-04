using DataStructures;
using PiCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using PiCross.Utility;

namespace PiCross
{
    internal class ExtendedPlayablePuzzle : PlayablePuzzle
    {
        public ExtendedPlayablePuzzle( string name, ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
            : this(name, new PlayGrid(columnConstraints: columnConstraints, rowConstraints: rowConstraints))
        {
                // NOP
        }

        public ExtendedPlayablePuzzle( string name, PlayGrid playGrid )
            :base( playGrid )
        {
            Name = Cell.Create( name );
            Mistakes = Cell.Create(0);
            // Timer?
            BestTime = Cell.Create( new TimeSpan() );
            Playable = Cell.Derived(Mistakes, m => m >= 5);

    }

        public Cell<string> Name { get; }
        public Cell<int> Mistakes { get; }
        public Cell<ITimer> Timer { get; set; }
        public Cell<TimeSpan> BestTime { get; }
        public Cell<bool> Playable { get; }

        public void StartTimer()
        {
            Timer.Value.Start();
        }

        public void StopTimer()
        {
            Timer.Value.Stop();
        }

        public void ResetTimer()
        {
            Timer.Value.Reset();
        }

    }
}
