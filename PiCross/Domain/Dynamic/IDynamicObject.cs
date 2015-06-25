using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PiCross.Dynamic
{    
    public interface IDynamicObject
    {
        ISet<string> Properties { get; }

        object this[string property] { get; }
    }

    public interface IDynamicObjectGroup<OBJECT> where OBJECT : IDynamicObject
    {
        ISet<string> Properties { get; }

        ISet<object> PropertyValues( string property );
    }

    public interface IFilter
    {
        bool Accept( IDynamicObject obj );
    }

    internal class DynamicObjectProxy<PROPERTY>
    {
        private IDynamicObject obj;

        public DynamicObjectProxy( IDynamicObject obj )
        {
            this.obj = obj;
        }

        [DebuggerBrowsable( DebuggerBrowsableState.RootHidden )]
        public PropertyValuePair[] Properties
        {
            get
            {
                return ( from property in this.obj.Properties
                         let value = this.obj[property]
                         select new PropertyValuePair( property, value ) ).ToArray();
            }
        }
    }

    [DebuggerDisplay("{Property} => {Value}")]
    internal class PropertyValuePair
    {
        private string property;

        private object value;

        public PropertyValuePair(string property, object value)
        {
            this.property = property;
            this.value = value;
        }

        public string Property
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
