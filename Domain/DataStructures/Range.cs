using System.Collections.Generic;
using System.Linq;

namespace DataStructures
{
    public class Range
    {
        private Range(int from, int length)
        {
            From = from;
            Length = length;
        }

        public int From { get; }

        public int Length { get; }

        public IEnumerable<int> Items
        {
            get { return Enumerable.Range(From, Length); }
        }

        public static Range FromStartAndLength(int start, int length)
        {
            return new Range(start, length);
        }

        public static Range FromStartAndEndExclusive(int start, int endExclusive)
        {
            return new Range(start, endExclusive - start);
        }

        public bool Contains(int n)
        {
            return From <= n && n < From + Length;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Range);
        }

        public bool Equals(Range that)
        {
            return that != null && From == that.From && Length == that.Length;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() ^ Length.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0}...{1})", From, From + Length);
        }
    }
}