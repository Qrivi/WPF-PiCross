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
        private readonly ISequence<SquareState> squares;

        public Slice( ISequence<SquareState> squares )
        {
            // TODO Validate

            this.squares = squares;
        }

        public ISequence<SquareState> Squares
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
    }
}
