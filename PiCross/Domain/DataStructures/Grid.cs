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
    }

    /// <summary>
    /// Extension methods for IGrid objects.
    /// </summary>
    public static class IGridExtensions
    {
        /// <summary>
        /// Checks if <paramref name="position"/> is valid.
        /// </summary>
        /// <typeparam name="T">Type of the items of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <param name="position">Position.</param>
        /// <returns>True if the position is valid for the grid, false otherwise.</returns>
        public static bool IsValidPosition<T>( this IGrid<T> grid, Vector2D position )
        {
            return 0 <= position.X && position.X < grid.Width && 0 <= position.Y && position.Y < grid.Height;
        }

        /// <summary>
        /// Enumerates all valid positions of the grid.
        /// </summary>
        /// <typeparam name="T">Type of the items in the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <returns>All valid positions of the grid. No specific order is guaranteed.</returns>
        public static IEnumerable<Vector2D> AllPositions<T>( this IGrid<T> grid )
        {
            return from y in Enumerable.Range( 0, grid.Height )
                   from x in Enumerable.Range( 0, grid.Width )
                   select new Vector2D( x, y );
        }

        /// <summary>
        /// Enumerates all row indices.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <returns>All row indices.</returns>
        public static IEnumerable<int> RowIndices<T>( this IGrid<T> grid )
        {
            return Enumerable.Range( 0, grid.Height );
        }

        /// <summary>
        /// Enumerates all column indices.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <returns>All column indices.</returns>
        public static IEnumerable<int> ColumnIndices<T>( this IGrid<T> grid )
        {
            return Enumerable.Range( 0, grid.Width );
        }

        private static int MaximumSteps( int x, int dx, int max )
        {
            if ( dx == -1 )
            {
                return x;
            }
            else if ( dx == 0 )
            {
                return int.MaxValue;
            }
            else if ( dx == 1 )
            {
                return max - x - 1;
            }
            else
            {
                throw new ArgumentOutOfRangeException( "dx" );
            }
        }

        /// <summary>
        /// Constructs a new grid of the same dimensions as the given grid
        /// whose items are equal to the result of applying <paramref name="function"/> to the
        /// item in the same position.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the original grid.</typeparam>
        /// <typeparam name="R">Type of the elements of the produced grid.</typeparam>
        /// <param name="grid">Original grid.</param>
        /// <param name="function">Function used to transform items from the original grid.</param>
        /// <returns>Grid.</returns>
        public static IGrid<R> Map<T, R>( this IGrid<T> grid, Func<T, R> function )
        {
            return new Grid<R>( grid.Width, grid.Height, p => function( grid[p] ) );
        }

        /// <summary>
        /// Constructs a grid with the same size as the given grid but whose
        /// items are equal to the result of applying <paramref name="function"/>
        /// to the corresponding item in the original grid. The difference
        /// between <see cref="Map"/> and <see cref="VirtualMap"/> is
        /// that the former creates a new grid in memory which is completely
        /// separate from the original grid, whereas the latter
        /// remains synchronized with the original grid. This means
        /// that changes to the original grid are visible in the grid returned
        /// by this method.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the original grid.</typeparam>
        /// <typeparam name="R">Type of the elements of the generated grid.</typeparam>
        /// <param name="grid">Original grid.</param>
        /// <param name="function">Function to be applied on the items of the original grid.</param>
        /// <returns>A virtual mapping of the original grid.</returns>
        public static IGrid<R> VirtualMap<T, R>( this IGrid<T> grid, Func<T, R> function )
        {
            return new VirtualGrid<R>( grid.Width, grid.Height, p => function( grid[p] ) );
        }

        /// <summary>
        /// Applies <paramref name="action"/> to each position in the grid.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <param name="action">Action to perform on each position.</param>
        public static void EachPosition<T>( this IGrid<T> grid, Action<Vector2D> action )
        {
            foreach ( var position in grid.AllPositions() )
            {
                action( position );
            }
        }

        /// <summary>
        /// Applies <paramref name="action"/> to each item in the grid.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <param name="action">Action to perform on each iten.</param>
        public static void Each<T>( this IGrid<T> grid, Action<T> action )
        {
            grid.EachPosition( p => action( grid[p] ) );
        }

        /// <summary>
        /// Creates a vertically flipped view of the original grid. The returned grid
        /// remains synchronized with the original grid.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <returns>Vertically flipped viez of the original grid.</returns>
        public static IGrid<T> FlipVertically<T>( this IGrid<T> grid )
        {
            return new VirtualGrid<T>( grid.Width, grid.Height, p => grid[new Vector2D( p.X, grid.Height - p.Y - 1 )] );
        }

        /// <summary>
        /// Creates a horizontally flipped view of the original grid. The returned grid
        /// remains synchronized with the original grid.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <returns>Horizontally flipped viez of the original grid.</returns>
        public static IGrid<T> FlipHorizontally<T>( this IGrid<T> grid )
        {
            return new VirtualGrid<T>( grid.Width, grid.Height, p => grid[new Vector2D( grid.Width - p.X - 1, p.Y )] );
        }

        /// <summary>
        /// Counts the number of items that satisfy <paramref name="predicate"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="grid">Grid.</param>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Number of items that satisfy <paramref name="predicate"/></returns>
        public static int Count<T>( this IGrid<T> grid, Func<T, bool> predicate )
        {
            var count = 0;

            grid.Each( x => count += ( predicate( x ) ? 1 : 0 ) );

            return count;
        }
    }

    public static class Grid
    {
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
                    return xss.AllPositions().All( p => xss[p] == null ? yss[p] == null : xss[p].Equals( yss[p] ) );
                }
                else
                {
                    return false;
                }
            }
        }

        internal static int HashCode<T>( IGrid<T> grid )
        {
            int result = 0;

            grid.Each( x => result ^= x.GetHashCode() );

            return result;
        }
    }

    public abstract class GridBase<T> : IGrid<T>
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
    }

    public class Grid<T> : GridBase<T>
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

    public class VirtualGrid<T> : GridBase<T>
    {
        private readonly Func<Vector2D, T> function;
        private readonly int width;
        private readonly int height;

        public VirtualGrid( int width, int height, Func<Vector2D, T> function )
        {
            this.function = function;
            this.width = width;
            this.height = height;
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
