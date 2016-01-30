using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;

namespace PiCross.Game
{
    public class PuzzleUID
    {
        private ISequence<byte> bytes;

        public static PuzzleUID FromPuzzle( Puzzle puzzle )
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
                var bytes = sizeBytes.Concatenate( gridBytes );

                return new PuzzleUID( bytes );
            }
        }

        public static PuzzleUID FromBase64(string str)
        {
            var bytes = Convert.FromBase64String( str ).ToSequence();

            return new PuzzleUID( bytes );
        }

        private PuzzleUID( ISequence<byte> bytes )
        {
            this.bytes = bytes;
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as PuzzleUID );
        }

        public bool Equals( PuzzleUID uid )
        {
            return uid != null && this.bytes.Equals( uid.bytes );
        }

        public override int GetHashCode()
        {
            return bytes.GetHashCode();
        }

        public override string ToString()
        {
            return bytes.Map( x => string.Format( "{0:x}", x ) ).Join( " " );
        }

        public string Base64
        {
            get
            {
                return Convert.ToBase64String( bytes.ToArray() );
            }
        }

        public Puzzle CreatePuzzle()
        {
            var width = this.bytes[0];
            var height = this.bytes[1];
            var gridBytes = this.bytes.DropPrefix( 2 ).Map( Sequence.Bits ).Flatten();
            var rowBits = gridBytes.Group( width ).Prefix( height );
            var grid = Grid.FromRows( rowBits );

            return Puzzle.FromGrid( grid );
        }
    }
}
