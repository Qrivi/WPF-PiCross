﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross.Game;

namespace PiCross.Facade
{
    public class PiCrossFacade
    {
        public IGameData CreateEmptyGameData()
        {
            return new GameData( PuzzleLibrary.CreateEmpty(), new PlayerDatabase() );
        }

        public IGameData LoadGameData( string path )
        {
            var io = new GameDataIO();

            using ( var fileStream = new FileStream( path, FileMode.Open ) )
            {
                return io.Read( fileStream );
            }
        }
    }
}
