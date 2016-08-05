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
        public ExtendedPlayablePuzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
            : this( new PlayGrid(columnConstraints: columnConstraints, rowConstraints: rowConstraints))
        {
                // NOP
        }

        public ExtendedPlayablePuzzle( PlayGrid playGrid )
            :base( playGrid )
        {
            Mistakes = Cell.Create(0);
            //IsPlayable = Cell.Derived<int, bool, bool>(Mistakes, IsSolved,( Mistakes.Value >= 5 && IsSolved.Value == false));
            IsPlayable = Cell.Derived(Mistakes, m => m >= 5);
        }
        
        public Cell<int> Mistakes { get; }
        public Cell<bool> IsPlayable { get; }
    }
}
