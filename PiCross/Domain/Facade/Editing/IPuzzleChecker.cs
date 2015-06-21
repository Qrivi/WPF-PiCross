using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.Editing
{
    public interface IPuzzleChecker
    {
        void FindAmbiguities( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, Cell<IGrid<bool>> output );

        void Kill();
    }
}
