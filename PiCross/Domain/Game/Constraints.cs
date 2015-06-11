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
            return GeneratePatterns( sliceLength, values ).Select( x => new Slice( x ) );
        }

        private static IEnumerable<ISequence<int>> GenerateIntegers( int count, int sum )
        {
            // TODO Validate arguments

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

        private static IEnumerable<ISequence<int>> GenerateSpacings( int length, int constraintCount, int constraintSum )
        {
            // TODO Validate arguments

            var spacingCount = constraintCount + 1;
            var spacingSum = length - constraintSum - Math.Max( 0, constraintCount - 1 );

            var numbers = GenerateIntegers( spacingCount, spacingSum );
            var deltas = Sequence.FromFunction( spacingCount, i => i == 0 || i == spacingCount - 1 ? 0 : 1 );

            return numbers.Select( ns => ns.ZipWith( deltas, ( x, y ) => x + y ) );
        }

        private static IEnumerable<ISequence<SquareState>> GeneratePatterns( int totalSize, ISequence<int> constraints )
        {
            if ( totalSize < 0 )
            {
                throw new ArgumentOutOfRangeException( "totalSize" );
            }
            else if ( constraints == null )
            {
                throw new ArgumentNullException( "constraints" );
            }
            else
            {
                var constraintCount = constraints.Length;
                var constraintSum = constraints.Items.Sum();

                var spacings = GenerateSpacings( totalSize, constraintCount: constraintCount, constraintSum: constraintSum );

                var blocks = constraints.Map( n => Sequence.Repeat( n, SquareState.FILLED ) );

                foreach ( var spacing in spacings )
                {
                    var spaces = spacing.Map( n => Sequence.Repeat( n, SquareState.EMPTY ) );

                    yield return spaces.Intersperse( blocks ).Flatten();
                }
            }
        }
    }
}
