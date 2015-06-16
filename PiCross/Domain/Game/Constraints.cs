using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;
using System.Diagnostics;

namespace PiCross.Game
{
    public class Constraints
    {
        private readonly ISequence<int> values;

        public Constraints( ISequence<int> values )
        {
            if ( values == null )
            {
                throw new ArgumentNullException( "values" );
            }
            else if ( values.Items.Any( n => n <= 0 ) )
            {
                throw new ArgumentOutOfRangeException( "values must all be strictly positive" );
            }
            else
            {
                this.values = values;
            }
        }

        public Constraints( params int[] values )
            : this( Sequence.FromItems( values ) )
        {
            // NOP
        }

        public Constraints( IEnumerable<int> values )
            : this( values.ToArray() )
        {
            // NOP
        }

        public IEnumerable<Slice> GenerateSlices( int sliceLength )
        {
            return GeneratePatterns( sliceLength, values ).Select( x => new Slice( x ) );
        }

        private static IEnumerable<ISequence<int>> GenerateIntegers( int count, int sum )
        {
            Debug.Assert( count >= 0 );
            Debug.Assert( sum >= 0 );

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
            Debug.Assert( length >= 0 );
            Debug.Assert( constraintCount >= 0 );
            Debug.Assert( constraintSum >= 0 );

            var spacingCount = constraintCount + 1;
            var spacingSum = length - constraintSum - Math.Max( 0, constraintCount - 1 );

            if ( spacingSum < 0 )
            {
                return Enumerable.Empty<ISequence<int>>();
            }
            else
            {
                var numbers = GenerateIntegers( spacingCount, spacingSum );
                var deltas = Sequence.FromFunction( spacingCount, i => i == 0 || i == spacingCount - 1 ? 0 : 1 );

                return numbers.Select( ns => ns.ZipWith( deltas, ( x, y ) => x + y ) );
            }
        }

        private static IEnumerable<ISequence<Square>> GeneratePatterns( int totalSize, ISequence<int> constraints )
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

                var blocks = constraints.Map( n => Sequence.Repeat( n, Square.FILLED ) );

                foreach ( var spacing in spacings )
                {
                    var spaces = spacing.Map( n => Sequence.Repeat( n, Square.EMPTY ) );

                    yield return spaces.Intersperse( blocks ).Flatten();
                }
            }
        }

        public int SatisfiedPrefixLength( Slice slice )
        {
            var knownPrefix = slice.KnownPrefix;
            var knownConstraints = knownPrefix.DeriveConstraints();

            return this.values.CommonPrefixLength( knownConstraints.values );
        }

        public Constraints Reverse()
        {
            return new Constraints( this.values.Reverse() );
        }

        public int SatisfiedSuffixLength( Slice slice )
        {
            return this.Reverse().SatisfiedPrefixLength( slice.Reverse() );
        }

        public ISequence<int> Values
        {
            get
            {
                return this.values;
            }
        }

        public bool IsSatisfied( Slice slice )
        {
            if ( slice.IsFullyKnown )
            {
                var derivedConstraints = slice.DeriveConstraints();

                return derivedConstraints.Equals( this );
            }
            else
            {
                return false;
            }
        }

        public Range UnsatisfiedValueRange( Slice slice )
        {
            var knownPrefix = slice.KnownPrefix;
            var rest = slice.Lift( xs => xs.DropPrefix( knownPrefix.Squares.Length ) );
            var knownSuffix = rest.KnownSuffix;

            var left = this.SatisfiedPrefixLength( knownPrefix );
            var right = this.values.Length - this.Lift( x => x.DropPrefix( left ) ).SatisfiedSuffixLength( knownSuffix );

            return Range.FromStartAndEndExclusive( left, right );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Constraints );
        }

        public bool Equals( Constraints that )
        {
            return that != null && this.values.Equals( that.values );
        }

        public override int GetHashCode()
        {
            return this.values.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format( "Constraints[{0}]", this.values.Map( x => x.ToString() ).Join( "-" ) );
        }

        public Constraints Lift( Func<ISequence<int>, ISequence<int>> function )
        {
            return new Constraints( function( values ) );
        }
    }
}
