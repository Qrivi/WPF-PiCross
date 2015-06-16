using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    public interface ISequence<out T>
    {
        int Length { get; }

        T this[int index] { get; }

        IEnumerable<int> Indices { get; }

        IEnumerable<T> Items { get; }

        bool IsValidIndex( int index );
    }

    public static class Sequence
    {
        public static ISequence<T> CreateEmpty<T>()
        {
            return new EmptySequence<T>();
        }

        public static ISequence<T> FromItems<T>( params T[] items )
        {
            return new ArraySequence<T>( items.Length, i => items[i] );
        }

        public static ISequence<T> FromFunction<T>( int length, Func<int, T> function )
        {
            return new VirtualSequence<T>( length, function );
        }

        public static ISequence<T> FromEnumerable<T>( IEnumerable<T> xs )
        {
            return FromItems( xs.ToArray() );
        }

        public static ISequence<T> Repeat<T>( int length, T value )
        {
            return new VirtualSequence<T>( length, _ => value );
        }        

        public static ISequence<char> FromString( string str )
        {
            return FromItems( str.ToCharArray() );
        }

        public static ISequence<int> Range(int from, int length)
        {
            return Sequence.FromFunction( length, i => from + i );
        }        
    }

    public static class SequenceExtensions
    {
        public static ISequence<T> ToSequence<T>( this IEnumerable<T> enumerable )
        {
            return Sequence.FromEnumerable( enumerable );
        }

        public static ISequence<T> Concatenate<T>( this ISequence<T> xs, ISequence<T> ys )
        {
            return Sequence.FromFunction( xs.Length + ys.Length, i => i < xs.Length ? xs[i] : ys[i - xs.Length] );
        }

        public static ISequence<T> Flatten<T>( this ISequence<ISequence<T>> xss )
        {
            if ( xss.Length == 0 )
            {
                return Sequence.CreateEmpty<T>();
            }
            else if (xss.Length == 1 )
            {
                return xss[0];
            }
            else
            {
                var middle = xss.Length / 2;

                var leftHalf = xss.Prefix( middle );
                var rightHalf = xss.DropPrefix( middle );

                return Flatten( leftHalf ).Concatenate( Flatten( rightHalf ) );
            }
        }

        public static ISequence<R> ZipWith<T1, T2, R>( this ISequence<T1> xs, ISequence<T2> ys, Func<T1, T2, R> zipper )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( "xs" );
            }
            else if ( ys == null )
            {
                throw new ArgumentNullException( "ys" );
            }
            else if ( xs.Length != ys.Length )
            {
                throw new ArgumentException( "xs and ys should have same length" );
            }
            else if ( zipper == null )
            {
                throw new ArgumentNullException( "zipper" );
            }
            else
            {
                return Sequence.FromFunction( xs.Length, i => zipper( xs[i], ys[i] ) );
            }
        }

        public static ISequence<R> Map<T, R>( this ISequence<T> xs, Func<T, R> function )
        {
            // TODO Argument validation

            return Sequence.FromFunction( xs.Length, i => function( xs[i] ) );
        }

        public static ISequence<T> Intersperse<T>( this ISequence<T> xs, ISequence<T> ys )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( "xs" );
            }
            else if ( ys == null )
            {
                throw new ArgumentNullException( "ys" );
            }
            else if ( xs.Length != ys.Length + 1 )
            {
                throw new ArgumentException( string.Format( "xs.Length (={0}) should be equal to ys.Length + 1 (={1} + 1)", xs.Length, ys.Length ) );
            }
            else
            {
                return Sequence.FromFunction( xs.Length + ys.Length, i => i % 2 == 0 ? xs[i / 2] : ys[( i - 1 ) / 2] );
            }
        }

        public static string AsString( this ISequence<char> xs )
        {
            return new string( xs.Items.ToArray() );
        }

        public static ISequence<T> Subsequence<T>( this ISequence<T> xs, int from, int count )
        {
            if ( !xs.IsValidIndex( from ) )
            {
                throw new ArgumentOutOfRangeException( "from" );
            }
            else if ( count < 0 || ( count > 0 && from + count - 1 >= xs.Length ) )
            {
                throw new ArgumentOutOfRangeException( "count" );
            }
            else
            {
                return Sequence.FromFunction( count, i => xs[i + from] );
            }
        }

        public static ISequence<T> Prefix<T>( this ISequence<T> xs, int length )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( "xs" );
            }
            else if ( length > xs.Length )
            {
                throw new ArgumentOutOfRangeException( "length" );
            }
            else
            {
                return xs.Subsequence( 0, length );
            }
        }

        public static ISequence<T> Suffix<T>( this ISequence<T> xs, int from )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( "xs" );
            }
            else if ( from > xs.Length )
            {
                throw new ArgumentOutOfRangeException( "from", string.Format( "from = {0}, length = {1}", from, xs.Length ) );
            }
            else
            {
                if ( from == xs.Length )
                {
                    return Sequence.CreateEmpty<T>();
                }
                else
                {
                    return xs.Subsequence( from, xs.Length - from );
                }
            }
        }

        public static ISequence<T> DropPrefix<T>( this ISequence<T> xs, int length )
        {
            return xs.Suffix( length );
        }

        public static ISequence<T> DropSuffix<T>( this ISequence<T> xs, int length )
        {
            return xs.Prefix( xs.Length - length - 1 );
        }

        public static int FindFirstIndexOf<T>( this ISequence<T> xs, Func<T, bool> predicate )
        {
            var i = 0;

            while ( i < xs.Length )
            {
                var current = xs[i];

                if ( predicate( current ) )
                {
                    return i;
                }

                ++i;
            }

            return -1;
        }

        public static ISequence<T> TakeWhile<T>( this ISequence<T> xs, Func<T, bool> predicate )
        {
            var index = xs.FindFirstIndexOf( x => !predicate( x ) );

            if ( index == -1 )
            {
                return xs;
            }
            else
            {
                return xs.Prefix( index );
            }
        }

        public static ISequence<T> DropWhile<T>( this ISequence<T> xs, Func<T, bool> predicate )
        {
            var index = xs.FindFirstIndexOf( x => !predicate( x ) );

            if ( index == -1 )
            {
                return Sequence.CreateEmpty<T>();
            }
            else
            {
                return xs.Suffix( index );
            }
        }

        public static ISequence<T> Reverse<T>( this ISequence<T> xs )
        {
            return Sequence.FromFunction( xs.Length, i => xs[xs.Length - i - 1] );
        }

        public static int CommonPrefixLength<T>( this ISequence<T> xs, ISequence<T> ys )
        {
            var i = 0;

            while ( i < xs.Length && i < ys.Length && xs[i].Equals( ys[i] ) )
            {
                ++i;
            }

            return i;
        }

        public static ISequence<T> Copy<T>( this ISequence<T> xs )
        {
            return new ArraySequence<T>( xs.Length, i => xs[i] );
        }

        public static string Join( this ISequence<char> cs, string infix = "" )
        {
            return string.Join( infix, cs.Items.ToArray() );
        }

        public static string Join( this ISequence<string> cs, string infix = "" )
        {
            return string.Join( infix, cs.Items.ToArray() );
        }
    }

    internal abstract class SequenceBase<T> : ISequence<T>
    {
        public abstract int Length { get; }

        public abstract T this[int index] { get; }

        public IEnumerable<int> Indices
        {
            get
            {
                return Enumerable.Range( 0, this.Length );
            }
        }

        public IEnumerable<T> Items
        {
            get
            {
                return Indices.Select( i => this[i] );
            }
        }

        public bool IsValidIndex( int index )
        {
            return 0 <= index && index < Length;
        }

        public override string ToString()
        {
            var items = string.Join( ", ", Items.Select( x => x.ToString() ) );

            return string.Format( "SEQ[{0}]", items );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as ISequence<T> );
        }

        public bool Equals( ISequence<T> that )
        {
            if ( that == null )
            {
                return false;
            }
            else if ( this.Length != that.Length )
            {
                return false;
            }
            else
            {
                return Indices.All( i => EqualItems( this[i], that[i] ) );
            }
        }

        private bool EqualItems( T x, T y )
        {
            if ( x == null )
            {
                return y == null;
            }
            else
            {
                return x.Equals( y );
            }
        }

        public override int GetHashCode()
        {
            return Items.Select( x => x.GetHashCode() ).Aggregate( 0, ( x, y ) => x ^ y );
        }
    }

    internal class EmptySequence<T> : SequenceBase<T>
    {
        public override int Length
        {
            get { return 0; }
        }

        public override T this[int index]
        {
            get { throw new ArgumentOutOfRangeException( "index" ); }
        }
    }

    internal class ArraySequence<T> : SequenceBase<T>
    {
        private readonly T[] items;

        public ArraySequence( int length, Func<int, T> initializer )
        {
            if ( length < 0 )
            {
                throw new ArgumentOutOfRangeException( "length" );
            }
            else if ( initializer == null )
            {
                throw new ArgumentNullException( "initializer" );
            }
            else
            {
                items = Enumerable.Range( 0, length ).Select( initializer ).ToArray();
            }
        }

        public override int Length
        {
            get
            {
                return items.Length;
            }
        }

        public override T this[int index]
        {
            get
            {
                return items[index];
            }
        }
    }

    internal class VirtualSequence<T> : SequenceBase<T>
    {
        private readonly int length;

        private readonly Func<int, T> function;

        public VirtualSequence( int length, Func<int, T> function )
        {
            if ( length < 0 )
            {
                throw new ArgumentOutOfRangeException( "length" );
            }
            else if ( function == null )
            {
                throw new ArgumentNullException( "function" );
            }
            else
            {
                this.length = length;
                this.function = function;
            }
        }

        public override int Length
        {
            get
            {
                return length;
            }
        }

        public override T this[int index]
        {
            get
            {
                if ( !IsValidIndex( index ) )
                {
                    throw new ArgumentOutOfRangeException( "index" );
                }
                else
                {
                    return function( index );
                }
            }
        }
    }
}
