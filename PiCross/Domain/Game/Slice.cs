using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PiCross.Game
{
    public class Slice
    {
        private readonly ISequence<Square> squares;

        public Slice( ISequence<Square> squares )
        {
            if ( squares == null )
            {
                throw new ArgumentNullException( "squares" );
            }
            else
            {
                this.squares = squares;
            }
        }

        public ISequence<Square> Squares
        {
            get
            {
                return squares;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Slice );
        }

        public bool Equals( Slice that )
        {
            if ( that == null )
            {
                return false;
            }
            else
            {
                return this.squares.Equals( that.squares );
            }
        }

        public override int GetHashCode()
        {
            return squares.GetHashCode();
        }

        public override string ToString()
        {
            return squares.Map( x => x.Symbol ).AsString();
        }

        public bool CompatibleWith( Slice that )
        {
            if ( that == null )
            {
                throw new ArgumentNullException( "slice" );
            }
            else if ( this.Squares.Length != that.Squares.Length )
            {
                throw new ArgumentException( "Slices should have same length" );
            }
            else
            {
                return this.squares.Indices.All( i => squares[i].CompatibleWith( that.squares[i] ) );
            }
        }

        public Slice Merge( Slice that )
        {
            if ( that == null )
            {
                throw new ArgumentNullException( "slice" );
            }
            else if ( this.Squares.Length != that.Squares.Length )
            {
                throw new ArgumentException( "Slices should have same length" );
            }
            else
            {
                return new Slice( this.squares.ZipWith( that.squares, ( x, y ) => x.Merge( y ) ) );
            }
        }

        public static Slice FromString( string str )
        {
            return new Slice( Sequence.FromString( str ).Map( Square.FromSymbol ) );
        }

        public static Slice Merge( IEnumerable<Slice> slices )
        {
            return slices.Aggregate( ( x, y ) => x.Merge( y ) );
        }

        public Slice Refine( Constraints constraints )
        {
            if ( constraints == null )
            {
                throw new ArgumentNullException( "constraints" );
            }
            else
            {
                return Merge( constraints.GenerateSlices( squares.Length ).Where( CompatibleWith ) );
            }
        }

        public ISequence<Range> FindBlocks()
        {
            var blocks = new List<Range>();
            var start = -1;

            var squares = this.squares.Concatenate( Sequence.FromItems( Square.EMPTY ) );

            for ( var i = 0; i != this.squares.Length; ++i )
            {
                var square = this.squares[i];

                Debug.Assert( square != null );

                if ( square == Square.UNKNOWN )
                {
                    throw new InvalidOperationException( "Slice must be fully known" );
                }
                else if ( square == Square.EMPTY )
                {
                    if ( start != -1 )
                    {
                        blocks.Add( Range.FromStartAndEndExclusive( start, i - 1 ) );
                        start = -1;
                    }
                }
                else // square == Square.FILLED
                {
                    if ( start == -1 )
                    {
                        start = i;
                    }
                }
            }

            return Sequence.FromEnumerable( blocks );
        }

        public Constraints DeriveConstraints()
        {
            var fillCount = 0;
            var constraints = new List<int>();

            for ( var i = 0; i != this.squares.Length; ++i )
            {
                var square = this.squares[i];

                if ( square == Square.FILLED )
                {
                    fillCount++;
                }
                else if ( square == Square.EMPTY )
                {
                    if ( fillCount > 0 )
                    {
                        constraints.Add( fillCount );
                    }

                    fillCount = 0;
                }
                else
                {
                    throw new InvalidOperationException( "Slice contained invalid square" );
                }
            }

            if ( fillCount > 0 )
            {
                constraints.Add( fillCount );
            }

            return new Constraints( constraints );
        }

        public bool IsFullyKnown
        {
            get
            {
                return squares.Items.All( x => x != Square.UNKNOWN );
            }
        }

        public Slice KnownPrefix
        {
            get
            {
                var known = this.squares.TakeWhile( sqr => sqr != Square.UNKNOWN );

                if ( known.Length == this.squares.Length )
                {
                    return this;
                }
                else
                {
                    var sqrs = known.Reverse().DropWhile( sqr => sqr == Square.FILLED ).Reverse();

                    return new Slice( sqrs );
                }
            }
        }

        public Slice KnownSuffix
        {
            get
            {
                return Reverse().KnownPrefix.Reverse();
            }
        }

        public Slice Reverse()
        {
            return Lift( ns => ns.Reverse() );
        }

        public Slice Prefix( int length )
        {
            return Lift( ns => ns.Prefix( length ) );
        }

        public Slice Suffix( int length )
        {
            return Lift( ns => ns.Suffix( length ) );
        }

        public Slice Lift( Func<ISequence<Square>, ISequence<Square>> function )
        {
            return new Slice( function( squares ) );
        }
    }
}
