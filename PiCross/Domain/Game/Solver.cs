using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Game
{
    public class Solver
    {
        internal static IEnumerable<ISequence<int>> GenerateIntegers(int count, int sum)
        {
            if ( count == 0 )
            {
                if ( sum == 0 )
                {
                    return new List<ISequence<int>> { Sequence.CreateEmpty<int>() };
                }
                else
                {
                    return Enumerable.Empty<ISequence<int>>();
                }
            }
            else
            {
                return from i in Enumerable.Range( 0, sum + 1 )
                       from tail in GenerateIntegers( count - 1, sum - i )
                       let prefix = Sequence.FromItems( i )
                       select prefix.Concatenate( tail );
            }
        }
    }
}
