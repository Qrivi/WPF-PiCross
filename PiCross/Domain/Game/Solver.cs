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
                // TODO Possible optimization: create separate sequence prefixer class
                return from i in Enumerable.Range( 0, sum + 1 )
                       from tail in GenerateIntegers( count - 1, sum - i )
                       let prefix = Sequence.FromItems( i )
                       select prefix.Concatenate( tail );
            }
        }

        internal static IEnumerable<ISequence<int>> GenerateSpacings(int length, int constraintCount, int constraintSum)
        {
            var spacingCount = constraintCount + 1;
            var spacingSum = length - constraintSum - Math.Max( 0, constraintCount - 2 );

            var numbers = GenerateIntegers( spacingCount, spacingSum );
            var deltas = Sequence.FromFunction( spacingCount, i => i == 0 || i == spacingCount - 1 ? 0 : 1 );

            return numbers.Select( ns => ns.ZipWith( deltas, ( x, y ) => x + y ) );
        }

        //internal static IEnumerable<ISequence<SquareState>> GeneratePatterns(int totalSize, ISequence<int> constraints)
        //{

        //}
    }
}
