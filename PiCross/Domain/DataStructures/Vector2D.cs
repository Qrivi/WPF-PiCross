using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    public class Vector2D
    {
        private readonly int x;
        private readonly int y;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        [DebuggerStepThrough]
        public Vector2D( int x = 0, int y = 0 )
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// X-coordinate
        /// </summary>
        public int X
        {
            get
            {
                return x;
            }
        }

        /// <summary>
        /// Y-coordinate
        /// </summary>
        public int Y
        {
            get
            {
                return y;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Vector2D );
        }

        public bool Equals( Vector2D v )
        {
            return !object.ReferenceEquals( v, null ) && this.x == v.x && this.y == v.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format( "({0}, {1})", x, y );
        }

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>Sum of both vectors</returns>
        public static Vector2D operator +( Vector2D u, Vector2D v )
        {
            if ( u == null )
            {
                throw new ArgumentNullException( "u" );
            }
            else if ( v == null )
            {
                throw new ArgumentNullException( "v" );
            }
            else
            {
                var x = u.x + v.x;
                var y = u.y + v.y;

                return new Vector2D( x, y );
            }
        }

        /// <summary>
        /// Negation.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>Negation of the given vector</returns>
        public static Vector2D operator -( Vector2D v )
        {
            if ( v == null )
            {
                throw new ArgumentNullException( "v" );
            }
            else
            {
                var x = -v.x;
                var y = -v.y;

                return new Vector2D( x, y );
            }
        }

        /// <summary>
        /// Subtraction.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>Difference</returns>
        public static Vector2D operator -( Vector2D u, Vector2D v )
        {
            if ( u == null )
            {
                throw new ArgumentNullException( "u" );
            }
            else if ( v == null )
            {
                throw new ArgumentNullException( "v" );
            }
            else
            {
                return u + ( -v );
            }
        }

        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="factor">Factor</param>
        /// <returns>Product</returns>
        public static Vector2D operator *( Vector2D v, int factor )
        {
            if ( v == null )
            {
                throw new ArgumentNullException( "v" );
            }
            else
            {
                var x = v.x * factor;
                var y = v.y * factor;

                return new Vector2D( x, y );
            }
        }

        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="factor">Factor</param>
        /// <param name="v">Vector</param>
        /// <returns>Product</returns>
        public static Vector2D operator *( int factor, Vector2D v )
        {
            return v * factor;
        }

        /// <summary>
        /// Checks for equality. Null is considered equal to null.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>True if both vectors are equal, false otherwise.</returns>
        public static bool operator ==( Vector2D u, Vector2D v )
        {
            if ( object.ReferenceEquals( u, null ) )
            {
                return object.ReferenceEquals( v, null );
            }
            else
            {
                return u.Equals( v );
            }
        }

        /// <summary>
        /// Checks for inequality.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>True if vectors are not equal, false otherwise.</returns>
        public static bool operator !=( Vector2D u, Vector2D v )
        {
            return !( u == v );
        }
    }
}
