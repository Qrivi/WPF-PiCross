using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    public interface IDynamicObject<PROPERTY>
    {
        IEnumerable<PROPERTY> Properties { get; }

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
}
