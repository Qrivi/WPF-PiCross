using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using DataStructures;

namespace PiCross
{
    public sealed class ExtendedPuzzle
    {
        public ExtendedPuzzle()
        {
            Name = Cell.Create("Nameless Puzzle");
            Puzzle = Cell.Create(PiCross.Puzzle.CreateEmpty(new Size(0,0)));
            BestTime = Cell.Create(new TimeSpan());
        }

        public Cell<string> Name { get; set; }
        public Cell<Puzzle> Puzzle { get; set; }
        public Cell<TimeSpan> BestTime { get; set; }
    }
}
