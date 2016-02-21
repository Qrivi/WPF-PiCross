using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross;

namespace PiCross
{
    public class PiCrossFacade
    {
        public IGameData CreateEmptyGameData()
        {
            var data = new InMemoryDatabase( InMemoryPuzzleLibrary.CreateEmpty(), InMemoryPlayerDatabase.CreateEmpty() );

            return new GameDataAdapter( data );
        }

        public IGameData CreateDummyGameData()
        {
            return new GameDataAdapter( DummyData.Create() );
        }

        public IGameData LoadGameData( string path )
        {
            var archive = new AutoCloseGameDataArchive( path );
            // var gameData = new ArchivedGameData( archive );
            var gameData = new GameDataAdapter( new ArchiveDatabase( path ) );

            return gameData;
        }

        public IPuzzleEditor CreatePuzzleEditor( Puzzle puzzle )
        {
            var editorGrid = EditorGrid.FromPuzzle( puzzle );
            var puzzleEditor = new PuzzleEditor( editorGrid );

            return puzzleEditor;
        }

        public IStepwisePuzzleSolver CreateStepwisePuzzleSolver( ISequence<Constraints> rowConstraints, ISequence<Constraints> columnConstraints )
        {
            var solverGrid = new SolverGrid( columnConstraints: columnConstraints, rowConstraints: rowConstraints );

            return new StepwiseSolver( solverGrid );
        }

        public IPlayablePuzzle CreatePlayablePuzzle( Puzzle puzzle )
        {
            return new PlayablePuzzle( columnConstraints: puzzle.ColumnConstraints, rowConstraints: puzzle.RowConstraints );
        }
    }
}
