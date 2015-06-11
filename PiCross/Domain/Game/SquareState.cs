using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Game
{
    public abstract class SquareState
    {
        public static readonly SquareState UNKNOWN = new Unknown();

        public static readonly SquareState FILLED = new Filled();

        public static readonly SquareState EMPTY = new Empty();

        public static SquareState FromSymbol( char symbol )
        {
            if ( UNKNOWN.Symbol == symbol )
            {
                return UNKNOWN;
            }
            else if ( FILLED.Symbol == symbol )
            {
                return FILLED;
            }
            else if ( EMPTY.Symbol == symbol )
            {
                return EMPTY;
            }
            else
            {
                throw new ArgumentOutOfRangeException( "symbol" );
            }
        }

        private SquareState()
        {
            // NOP
        }

        public abstract bool CompatibleWith( SquareState that );

        public abstract SquareState Merge( SquareState that );

        public abstract char Symbol { get; }

        public override bool Equals( object obj )
        {
            return this == obj;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }

        private class Unknown : SquareState
        {
            public override bool CompatibleWith( SquareState that )
            {
                return true;
            }

            public override SquareState Merge( SquareState that )
            {
                return UNKNOWN;
            }

            public override char Symbol
            {
                get { return '?'; }
            }
        }

        private class Filled : SquareState
        {
            public override bool CompatibleWith( SquareState that )
            {
                return that != EMPTY;
            }

            public override SquareState Merge( SquareState that )
            {
                if ( that == this )
                {
                    return this;
                }
                else
                {
                    return UNKNOWN;
                }
            }

            public override char Symbol
            {
                get { return 'x'; }
            }
        }

        private class Empty : SquareState
        {
            public override bool CompatibleWith( SquareState that )
            {
                return that != FILLED;
            }

            public override SquareState Merge( SquareState that )
            {
                if ( that == this )
                {
                    return this;
                }
                else
                {
                    return UNKNOWN;
                }
            }

            public override char Symbol
            {
                get { return '.'; }
            }
        }
    }
}
