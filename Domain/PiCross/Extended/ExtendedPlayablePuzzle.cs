using DataStructures;
using PiCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

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
            mistakes = Cell.Create(0);
            isPlayable = Cell.Derived(mistakes, isSolved, (m, s) =>
            {
                if (s)
                    return false;
                return m < 5;
            });
        }
    }
}
