using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    public class DynamicObjectMerger<PROPERTY> : IDynamicObject<PROPERTY>
    {
        private readonly IDynamicObject<PROPERTY> first;

        private readonly IDynamicObject<PROPERTY> second;

        public DynamicObjectMerger(IDynamicObject<PROPERTY> first, IDynamicObject<PROPERTY> second)
        {
            if ( first == null )
            {
                throw new ArgumentNullException( "first" );
            }
            else if ( second == null )
            {
                throw new ArgumentNullException( "second" );
            }
            else if ( first.Properties.Overlaps(second.Properties) )
            {
                throw new ArgumentException( "Objects must not share properties" );
            }
            else
            {
                this.first = first;
                this.second = second;
            }
        }

        public ISet<PROPERTY> Properties
        {
            get
            {
                var result = new HashSet<PROPERTY>(first.Properties);
                result.UnionWith( second.Properties );

                return result;
            }
        }

        public object this[PROPERTY property]
        {
            get
            {
                if ( first.Properties.Contains(property))
                {
                    return first[property];
                }
                else
                {
                    return second[property];
                }
            }
        }
    }
}
