using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.IO
{
    public class MagicCookieDecorator<T> : ISerializer<T>
    {
        private readonly string magicCookie;

        private readonly ISerializer<T> serializer;

        public MagicCookieDecorator( string magicCookie, ISerializer<T> serializer )
        {
            if ( magicCookie == null )
            {
                throw new ArgumentNullException( "magicCookie" );
            }
            else if ( serializer == null )
            {
                throw new ArgumentNullException( "serializer" );
            }
            else
            {
                this.magicCookie = magicCookie;
                this.serializer = serializer;
            }
        }

        public void Write( System.IO.StreamWriter writer, T obj )
        {
            writer.WriteLine( magicCookie );
            serializer.Write( writer, obj );
        }

        public T Read( System.IO.StreamReader reader )
        {
            var line = reader.ReadLine();

            if ( line != magicCookie )
            {
                throw new IOException();
            }
            else
            {
                return serializer.Read( reader );
            }
        }
    }
}
