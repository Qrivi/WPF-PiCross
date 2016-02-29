using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class Range
    {
        private readonly int from;

        private readonly int length;

        public static Range FromStartAndLength( int start, int length )
        {
            return new Range( start, length );
        }

        public static Range FromStartAndEndExclusive(int start, int endExclusive)
        {
            return new Range( start, endExclusive - start );
        }

        private Range( int from, int length )
        {
            this.from = from;
            this.length = length;
        }

        public int From
        {
            get
            {
                return from;
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public bool Contains(int n)
        {
            return from <= n && n < from + length;
        }

        public IEnumerable<int> Items
        {
            get
            {
                return Enumerable.Range( from, length );
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Range );
        }

        public bool Equals( Range that )
        {
            return that != null && this.from == that.from && this.length == that.length;
        }

        public override int GetHashCode()
        {
            return from.GetHashCode() ^ length.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format( "[{0}...{1})", from, from + length );
        }
    }
}
