using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;

namespace PiCross.Game
{
    public abstract class Square
    {
        public static readonly Square UNKNOWN = new Unknown();

        public static readonly Square FILLED = new Filled();

        public static readonly Square EMPTY = new Empty();

        public static Square FromSymbol( char symbol )
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
                throw new ArgumentException( string.Format("Unknown symbol: {0}", symbol ) );
            }
        }

        public static IGrid<Square> CreateGrid(params string[] rows)
        {
            return CreateGrid( Grid.CreateCharacterGrid( rows ) );
        }

        public static IGrid<Square> CreateGrid( IGrid<char> grid )
        {
            return grid.Map( Square.FromSymbol );
        }

        private Square()
        {
            // NOP
        }

        public abstract bool CompatibleWith( Square that );

        public abstract Square Merge( Square that );

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

        private class Unknown : Square
        {
            public override bool CompatibleWith( Square that )
            {
                return true;
            }

            public override Square Merge( Square that )
            {
                return UNKNOWN;
            }

            public override char Symbol
            {
                get { return '?'; }
            }
        }

        private class Filled : Square
        {
            public override bool CompatibleWith( Square that )
            {
                return that != EMPTY;
            }

            public override Square Merge( Square that )
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

        private class Empty : Square
        {
            public override bool CompatibleWith( Square that )
            {
                return that != FILLED;
            }

            public override Square Merge( Square that )
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
