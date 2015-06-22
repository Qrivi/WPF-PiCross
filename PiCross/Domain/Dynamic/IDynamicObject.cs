using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Cells;

namespace PiCross.Dynamic
{
    public interface IDynamicObject
    {
        IEnumerable<IDynamicProperty> Properties { get; }

        Cell<object> this[IDynamicProperty property] { get; }
    }

    public interface IDynamicProperty
    {
        string Name { get; }
    }

    public interface IDynamicObjectGroup
    {
        IEnumerable<IDynamicObject> Properties { get; }

        IEnumerable<object> PropertyValues( IDynamicProperty property );
    }

    public interface IFilter
    {
        bool Accept( IDynamicObject obj );
    }
}
