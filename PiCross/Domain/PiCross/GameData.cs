using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross;

namespace PiCross
{
    internal class GameData : IGameData
    {
        private readonly PuzzleLibrary library;

        private readonly InMemoryPlayerDatabase playerDatabase;

        public GameData(PuzzleLibrary library, InMemoryPlayerDatabase playerDatabase)
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

        IPuzzleLibrary IGameData.PuzzleLibrary
        {
            get
            {
                return Library;
            }
        }

        IPlayerDatabase IGameData.PlayerDatabase
        {
            get
            {
                return PlayerDatabase;
            }
        }

        public PuzzleLibrary Library
        {
            get
            {
                return library;
            }
        }

        public InMemoryPlayerDatabase PlayerDatabase
        {
            get
            {
                return playerDatabase;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as GameData );
        }

        public bool Equals(GameData gameData)
        {
            return gameData != null && library.Equals( gameData.library ) && playerDatabase.Equals( gameData.playerDatabase );
        }

        public override int GetHashCode()
        {
            return library.GetHashCode() ^ playerDatabase.GetHashCode();
        }
    }
}
