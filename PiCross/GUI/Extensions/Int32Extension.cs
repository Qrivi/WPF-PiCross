using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace GUI.Extensions {

    public class Int32Extension : MarkupExtension {

        public Int32Extension( int value ) { this.Value = value; }
        public int Value { get; set; }
        public override Object ProvideValue( IServiceProvider sp ) { return Value; }
    }
}
