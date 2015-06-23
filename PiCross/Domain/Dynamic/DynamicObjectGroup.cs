using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Dynamic
{
    public class DynamicObjectGroup<OBJECT, PROPERTY> : IDynamicObjectGroup<OBJECT, PROPERTY>
        where OBJECT : IDynamicObject<PROPERTY>
    {
        private readonly List<OBJECT> members;

        public static DynamicObjectGroup<OBJECT, PROPERTY> FromMembers( params OBJECT[] members )
        {
            return new DynamicObjectGroup<OBJECT, PROPERTY>( members.ToList() );
        }

        private DynamicObjectGroup( List<OBJECT> members )
        {
            this.members = members;
        }

        public ISet<PROPERTY> Properties
        {
            get
            {
                var result = new HashSet<PROPERTY>();

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

        public ISet<object> PropertyValues( PROPERTY property )
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
