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
            return new GameData( PuzzleLibrary.CreateEmpty(), PlayerDatabase.CreateEmpty() );
        }

        public IGameData CreateDummyGameData()
        {
            return DummyData.Create();
        }

        public IGameData LoadGameData( string path )
        {
            var io = new GameDataIO();

            using ( var fileStream = new FileStream( path, FileMode.Open ) )
            {
                return io.Read( fileStream );
            }
        }

        public IPuzzleEditor CreatePuzzleEditor(Puzzle puzzle)
        {
            var editorGrid = EditorGrid.FromPuzzle( puzzle );
            var puzzleEditor = new PuzzleEditor_ManualAmbiguity( editorGrid );

            return puzzleEditor;
        }
    }
}
