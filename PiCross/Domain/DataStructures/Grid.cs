using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    /// <summary>
    /// Interface for grids. A grid is immutable (i.e. readonly).
    /// If you need to be able to modify items in a grid, use <see cref="IVar" />.
    /// </summary>
    /// <typeparam name="T">Type of the items in the grid.</typeparam>
    public interface IGrid<out T>
    {
        /// <summary>
        /// Retrieves an item from the grid at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        T this[Vector2D position] { get; }

        /// <summary>
        /// Width of the grid.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the grid.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Checks if <paramref name="position"/> is valid.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>True if the position is valid for the grid, false otherwise.</returns>
        bool IsValidPosition( Vector2D position );

        /// <summary>
        /// Enumerates all valid positions of the grid.
        /// </summary>
        /// <returns>All valid positions of the grid. No specific order is guaranteed.</returns>
        IEnumerable<Vector2D> AllPositions { get; }

        /// <summary>
        /// Enumerates all row indices.
        /// </summary>
        /// <returns>All row indices.</returns>
        IEnumerable<int> RowIndices { get; }

        /// <summary>
        /// Enumerates all column indices.
        /// </summary>
        /// <returns>All column indices.</returns>
        IEnumerable<int> ColumnIndices { get; }

        IEnumerable<T> Items { get; }

        ISequence<T> Row( int index );

        ISequence<T> Column( int index );

        IEnumerable<ISequence<T>> Rows { get; }

        IEnumerable<ISequence<T>> Columns { get; }
    }

    /// <summary>
    /// Extension methods for IGrid objects.
    /// </summary>
    public static class IGridExtensions
    {
        public static IGrid<R> Map<T, R>(this IGrid<T> grid, Func<T, R> function)
        {
            return Grid.CreateVirtual( grid.Width, grid.Height, p => function( grid[p] ) );
        }

        public static IGrid<T> Copy<T>(this IGrid<T> grid)
        {
            return Grid.Create( grid.Width, grid.Height, p => grid[p] );
        }
    }

    public static class Grid
    {
        public static IGrid<T> Create<T>( int width, int height, Func<Vector2D, T> initializer )
        {
            return new Grid<T>( width, height, initializer );
        }

        public static IGrid<T> Create<T>( int width, int height, T initialValue = default(T) )
        {
            return Create( width, height, _ => initialValue );
        }

        public static IGrid<T> CreateVirtual<T>( int width, int height, Func<Vector2D, T> function )
        {
            return new VirtualGrid<T>( width, height, function );
        }

        public static IGrid<char> CreateCharacterGrid( params string[] strings )
        {
            var height = strings.Length;
            var width = strings[0].Length;

            if ( !strings.All( s => s.Length == width ) )
            {
                throw new ArgumentException( "All strings must have equal length" );
            }
            else
            {
                return new Grid<char>( width, height, p => strings[p.Y][p.X] );
            }
        }

        internal static bool EqualItems<T>( IGrid<T> xss, IGrid<T> yss )
        {
            if ( xss == null )
            {
                throw new ArgumentNullException( "xss" );
            }
            else if ( yss == null )
            {
                throw new ArgumentNullException( "yss" );
            }
            else
            {
                if ( xss.Width == yss.Width && xss.Height == yss.Height )
                {
                    return xss.AllPositions.All( p => xss[p] == null ? yss[p] == null : xss[p].Equals( yss[p] ) );
                }
                else
                {
                    return false;
                }
            }
        }

        internal static int HashCode<T>( IGrid<T> grid )
        {
            return grid.Items.Select( x => x.GetHashCode() ).Aggregate( 0, ( x, y ) => x ^ y );
        }
    }

    internal abstract class GridBase<T> : IGrid<T>
    {
        public override bool Equals( object obj )
        {
            return Equals( obj as IGrid<T> );
        }

        public bool Equals( IGrid<T> grid )
        {
            if ( grid == null )
            {
                return false;
            }
            else
            {
                return Grid.EqualItems( this, grid );
            }
        }

        public override int GetHashCode()
        {
            return Grid.HashCode( this );
        }

        public abstract T this[Vector2D position] { get; }

        public abstract int Width { get; }

        public abstract int Height { get; }

        public bool IsValidPosition( Vector2D position )
        {
            return 0 <= position.X && position.X < this.Width && 0 <= position.Y && position.Y < this.Height;
        }

        public IEnumerable<Vector2D> AllPositions
        {
            get
            {
                return from y in Enumerable.Range( 0, this.Height )
                       from x in Enumerable.Range( 0, this.Width )
                       select new Vector2D( x, y );
            }
        }

        public IEnumerable<int> RowIndices
        {
            get
            {
                return Enumerable.Range( 0, this.Height );
            }
        }

        public IEnumerable<int> ColumnIndices
        {
            get
            {
                return Enumerable.Range( 0, this.Width );
            }
        }

        public IEnumerable<T> Items
        {
            get
            {
                return AllPositions.Select( p => this[p] );
            }
        }

        public ISequence<T> Row( int y )
        {
            return Sequence.FromFunction( Width, x => this[new Vector2D( x, y )] );
        }

        public ISequence<T> Column( int x )
        {
            return Sequence.FromFunction( Height, y => this[new Vector2D( x, y )] );
        }

        public IEnumerable<ISequence<T>> Rows
        {
            get
            {
                return RowIndices.Select( Column );
            }
        }

        public IEnumerable<ISequence<T>> Columns
        {
            get
            {
                return ColumnIndices.Select( Row );
            }
        }
    }

    internal class Grid<T> : GridBase<T>
    {
        private readonly T[,] items;

        public Grid( int width, int height, Func<Vector2D, T> initializer )
        {
            items = new T[width, height];

            foreach ( var x in Enumerable.Range( 0, width ) )
            {
                foreach ( var y in Enumerable.Range( 0, height ) )
                {
                    var position = new Vector2D( x, y );

                    items[x, y] = initializer( position );
                }
            }
        }

        public Grid( int width, int height, T initialValue = default(T) )
            : this( width, height, p => initialValue )
        {
            // NOP
        }

        public override int Width
        {
            get
            {
                return items.GetLength( 0 );
            }
        }

        public override int Height
        {
            get
            {
                return items.GetLength( 1 );
            }
        }

        public override T this[Vector2D position]
        {
            get
            {
                return items[position.X, position.Y];
            }
        }
    }

    internal class VirtualGrid<T> : GridBase<T>
    {
        private readonly Func<Vector2D, T> function;

        private readonly int width;

        private readonly int height;

        public VirtualGrid( int width, int height, Func<Vector2D, T> function )
        {
            if ( width < 0 )
            {
                throw new ArgumentOutOfRangeException( "width" );
            }
            else if ( height < 0 )
            {
                throw new ArgumentOutOfRangeException( "height" );
            }
            else if ( function == null )
            {
                throw new ArgumentNullException( "function" );
            }
            else
            {
                this.function = function;
                this.width = width;
                this.height = height;
            }
        }

        public override int Width
        {
            get
            {
                return width;
            }
        }

        public override int Height
        {
            get
            {
                return height;
            }
        }

        public override T this[Vector2D position]
        {
            get
            {
                if ( !this.IsValidPosition( position ) )
                {
                    throw new ArgumentOutOfRangeException( "position" );
                }
                else
                {
                    return function( position );
                }
            }
        }
    }
}
