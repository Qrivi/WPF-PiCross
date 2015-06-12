using PiCross.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Game
{
    public class Slice
    {
        private readonly ISequence<Square> squares;

        public Slice( ISequence<Square> squares )
        {
            // TODO Validate

            this.squares = squares;
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

        public bool Equals(Slice that)
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

        public bool CompatibleWith(Slice that)
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

        public Slice Merge(Slice that)
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

        public static Slice FromString(string str)
        {
            return new Slice( Sequence.FromString( str ).Map( Square.FromSymbol ) );
        }

        public static Slice Merge(IEnumerable<Slice> slices)
        {
            return slices.Aggregate( ( x, y ) => x.Merge( y ) );
        }
        
        public Slice Refine(Constraints constraints)
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

        public Constraints DeriveConstraints()
        {
            var fillCount = 0;
            var constraints = new List<int>();
            
            foreach ( var square in this.squares.Items )
            {
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
    }
}
