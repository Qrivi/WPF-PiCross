using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PiCross.Dynamic
{    
    public interface IDynamicObject<PROPERTY>
    {
        ISet<PROPERTY> Properties { get; }

        object this[PROPERTY property] { get; }
    }

    public interface IDynamicObjectGroup<OBJECT, PROPERTY> where OBJECT : IDynamicObject<PROPERTY>
    {
        ISet<PROPERTY> Properties { get; }

        ISet<object> PropertyValues( PROPERTY property );
    }

    public interface IFilter<PROPERTY>
    {
        bool Accept( PROPERTY obj );
    }

    internal class DynamicObjectProxy<PROPERTY>
    {
        private IDynamicObject<PROPERTY> obj;

        public DynamicObjectProxy( IDynamicObject<PROPERTY> obj )
        {
            this.obj = obj;
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public PropertyValuePair<PROPERTY>[] Properties
        {
            get
            {
                return ( from property in this.obj.Properties
                         let value = this.obj[property]
                         select new PropertyValuePair<PROPERTY>( property, value ) ).ToArray();
            }
        }
    }

    [DebuggerDisplay("{Property} => {Value}")]
    internal class PropertyValuePair<PROPERTY>
    {
        private PROPERTY property;

        private object value;

        public PropertyValuePair(PROPERTY property, object value)
        {
            this.property = property;
            this.value = value;
        }

        public PROPERTY Property
        {
            get
            {
                return property;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
        }        
    }
}
