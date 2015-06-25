using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    public class DynamicObjectGroup<OBJECT> : IDynamicObjectGroup<OBJECT>
        where OBJECT : IDynamicObject
    {
        private readonly List<OBJECT> members;

        public static DynamicObjectGroup<OBJECT> FromMembers( params OBJECT[] members )
        {
            return new DynamicObjectGroup<OBJECT>( members.ToList() );
        }

        private DynamicObjectGroup( List<OBJECT> members )
        {
            this.members = members;
        }

        public ISet<string> Properties
        {
            get
            {
                var result = new HashSet<string>();

                foreach ( var member in members )
                {
                    foreach ( var property in member.Properties )
                    {
                        result.Add( property );
                    }
                }

                return result;
            }
        }

        public ISet<object> PropertyValues( string property )
        {
            var result = new HashSet<object>();

            foreach ( var member in members )
            {
                if ( member.Properties.Contains( property ) )
                {
                    result.Add( member[property] );
                }
            }

            return result;
        }
    }
}
