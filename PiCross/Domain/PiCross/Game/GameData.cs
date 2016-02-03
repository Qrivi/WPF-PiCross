using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiCross.Game
{
    internal class GameData
    {
        private readonly Library library;

        private readonly PlayerDatabase playerDatabase;

        public GameData(Library library, PlayerDatabase playerDatabase)
        {
            if ( library == null )
            {
                throw new ArgumentNullException( "library" );
            }
            else if ( playerDatabase == null )
            {
                throw new ArgumentNullException( "playerDatabase" );
            }
            else
            {
                this.library = library;
                this.playerDatabase = playerDatabase;
            }
        }

        public Library Library
        {
            get
            {
                return library;
            }
        }

        public PlayerDatabase PlayerDatabase
        {
            get
            {
                return playerDatabase;
            }
        }
    }
}
