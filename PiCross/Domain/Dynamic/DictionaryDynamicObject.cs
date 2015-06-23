using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    public class DictionaryDynamicObject<PROPERTY> : IDynamicObject<PROPERTY>, IEnumerable<KeyValuePair<PROPERTY, object>>
    {
        private readonly Dictionary<PROPERTY, object> contents;

        public DictionaryDynamicObject()
        {
            this.contents = new Dictionary<PROPERTY, object>();
        }

        public object this[PROPERTY property]
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

        public IEnumerable<PROPERTY> Properties
        {
            get
            {
                return contents.Keys;
            }
        }

        #region Necessary for collection initializers

        public void Add(PROPERTY property, object value)
        {
            this[property] = value;
        }

        public IEnumerator<KeyValuePair<PROPERTY, object>> GetEnumerator()
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
