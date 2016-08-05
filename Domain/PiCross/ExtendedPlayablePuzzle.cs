using Cells;
using DataStructures;

namespace PiCross
{
    internal class ExtendedPlayablePuzzle : PlayablePuzzle
    {
        public ExtendedPlayablePuzzle(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints)
            : this(new PlayGrid(columnConstraints, rowConstraints))
        {
            // NOP
        }

        public ExtendedPlayablePuzzle(PlayGrid playGrid)
            : base(playGrid)
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