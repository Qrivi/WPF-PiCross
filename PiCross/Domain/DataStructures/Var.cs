using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.DataStructures
{
    public class Var<T> : IVar<T>
    {
        private T value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialValue">
        /// Var's initial value.
        /// </param>
        public Var( T initialValue = default(T) )
        {
            this.value = initialValue;
        }

        /// <summary>
        /// Value of the Var.
        /// </summary>
        public virtual T Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public override string ToString()
        {
            return string.Format( "VAR[{0}]", value != null ? value.ToString() : "null" );
        }

        public override bool Equals( object obj )
        {
            throw new NotImplementedException( "Equals is not implemented for Vars" );
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException( "GetHashCode is not implemented for Vars" );
        }

        protected static bool AreEqual( T oldValue, T newValue )
        {
            if ( oldValue == null )
            {
                return newValue == null;
            }
            else
            {
                return oldValue.Equals( newValue );
            }
        }
    }
}
