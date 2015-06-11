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

        private SquareState()
        {
            // NOP
        }

        public abstract bool CompatibleWith(SquareState that);

        public abstract SquareState Merge( SquareState that );

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
        }
    }
}
