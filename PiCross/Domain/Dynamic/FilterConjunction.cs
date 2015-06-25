using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    public class FilterConjunction : IFilter
    {
        private readonly IEnumerable<IFilter> filters;

        public FilterConjunction(IEnumerable<IFilter> filters)
        {
            this.filters = filters.ToList();
        }

        public bool Accept( IDynamicObject obj )
        {
            return filters.All( filter => filter.Accept( obj ) );
        }
    }
}
