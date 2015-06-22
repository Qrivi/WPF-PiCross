using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross
{
    public class Util
    {
        public static bool AreEqual<T>(T x, T y)
        {
            if ( x == null )
            {
                return y == null;
            }
            else
            {
                return x.Equals( y );
            }
        }
    }
}
