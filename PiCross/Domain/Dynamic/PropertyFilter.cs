using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    public class PropertyFilter : IFilter
    {
        private readonly string property;

        private readonly ISet<object> acceptedValues;

        public PropertyFilter(string property, ISet<object> acceptedValues)
        {
            this.property = property;
            this.acceptedValues = new HashSet<object>( acceptedValues );
        }

        public bool Accept( IDynamicObject obj )
        {
            return obj.Properties.Contains( property ) && acceptedValues.Contains( obj[property] );
        }
    }
}
