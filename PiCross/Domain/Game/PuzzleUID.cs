using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.DataStructures;

namespace PiCross.Game
{
    public class PuzzleUID
    {
        private ISequence<byte> bytes;

        public PuzzleUID(Puzzle puzzle)
        {
            if ( puzzle == null )
            {
                throw new ArgumentNullException( "puzzle" );
            }
            else if ( puzzle.Size.Width > 255 )
            {
                throw new ArgumentOutOfRangeException( "Puzzle's width must not exceed 255" );
            }
            else if ( puzzle.Size.Height > 255 )
            {
                throw new ArgumentOutOfRangeException( "Puzzle's height must not exceed 255" );
            }
            else
            {
                this.bytes = puzzle.Grid.Linearize().GroupBits();
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as PuzzleUID );
        }

        public bool Equals(PuzzleUID uid)
        {
            return uid != null && this.bytes.Equals( uid.bytes );
        }

        public override int GetHashCode()
        {
            return bytes.GetHashCode();
        }

        public override string ToString()
        {
            return bytes.Map( x => string.Format( "{0:x}", x ) ).Join(" ");
        }
    }
}
