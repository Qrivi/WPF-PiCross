using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Game
{
    public class Constraints
    {
        private readonly ISequence<int> values;

        public Constraints( ISequence<int> values )
        {
            // TODO Validation

            this.values = values;
        }

        public IEnumerable<Slice> Generate( int sliceLength )
        {
            return Solver.GeneratePatterns( sliceLength, values ).Select( x => new Slice( x ) );
        }
    }
}
