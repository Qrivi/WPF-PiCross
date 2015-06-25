using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    [DebuggerDisplay("Property count = {PropertyCount}")]
    [DebuggerTypeProxy( typeof( DynamicObjectProxy<> ) )]
    public class DictionaryDynamicObject : IDynamicObject, IEnumerable<KeyValuePair<string, object>>
    {
        private readonly Dictionary<string, object> contents;

        public DictionaryDynamicObject()
        {
            this.contents = new Dictionary<string, object>();
        }

        public object this[string property]
        {
            get
            {
                if ( !contents.ContainsKey( property ) )
                {
                    throw new ArgumentException( string.Format( "Unknown property {0}", property ) );
                }
                else
                {
                    return contents[property];
                }
            }
            set
            {
                contents[property] = value;
            }
        }

        public ISet<string> Properties
        {
            get
            {
                return new HashSet<string>( contents.Keys );
            }
        }

        private int PropertyCount
        {
            get
            {
                return contents.Count;
            }
        }

        #region Necessary for collection initializers

        public void Add(string property, object value)
        {
            this[property] = value;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.contents.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.contents.GetEnumerator();
        }

        #endregion
    }
}
