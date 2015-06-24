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
            else
            {
                var sizeBytes = Sequence.FromItems<byte>( (byte) puzzle.Size.Width, (byte) puzzle.Size.Height );
                var gridBytes = puzzle.Grid.Linearize().GroupBits();

                this.bytes = sizeBytes.Concatenate( gridBytes );
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

        public Puzzle CreatePuzzle()
        {
            var width = this.bytes[0];
            var height = this.bytes[1];
            var gridBytes = this.bytes.DropPrefix( 2 ).Map( Sequence.Bits ).Flatten();
            var rowBits = gridBytes.Group( width ).Prefix(height);
            var grid = Grid.FromRows( rowBits );
            
            return Puzzle.FromGrid( grid );
        }
    }
}
