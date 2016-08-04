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
    /// <summary>
    /// Centralized access point to the domain classes.
    /// </summary>
    public class PiCrossFacade
    {
        /// <summary>
        /// Creates a IGameData object that contains no puzzles or players.
        /// Everything is kept in memory, i.e. data will never be persisted to file.
        /// </summary>
        /// <returns>An IGameData object.</returns>
        public IGameData CreateEmptyGameData()
        {
            var data = new InMemoryDatabase( InMemoryDatabase.PuzzleLibrary.CreateEmpty(), InMemoryDatabase.PlayerDatabase.CreateEmpty() );

            return new GameDataAdapter( data );
        }

        /// <summary>
        /// Creates dummy game data. Feel free to adapt <see cref="DummyData" /> to suit your needs.
        /// This data is kept in memory; in other words, it will never be persisted to file.
        /// </summary>
        /// <returns>An IGameData object.</returns>
        public IGameData CreateDummyGameData()
        {
            return new GameDataAdapter( DummyData.Create() );
        }

        /// <summary>
        /// Loads data from the given file.
        /// When the data is modified, changes are automatically written to the file.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>An IGameData object.</returns>
        public IGameData LoadGameData( string path )
        {
            var archive = new AutoCloseGameDataArchive( path );
            var gameData = new GameDataAdapter( new ArchiveDatabase( path ) );

            return gameData;
        }

        /// <summary>
        /// Creates a puzzle editor. This object offers
        /// all functionality related to the creation and modification
        /// of puzzles.
        /// </summary>
        /// <param name="puzzle">Puzzle to be edited. Note that this object cannot be changed; the editor
        /// uses this puzzle solely as an initial state.</param>
        /// <returns>An IPuzzleEditor object.</returns>
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

        /// <summary>
        /// Creates a playable puzzle. This object
        /// offers all functionality related to the solving of a puzzle.
        /// </summary>
        /// <param name="puzzle">Puzzle to be solved.</param>
        /// <returns>An IPlayablePuzzle object.</returns>
        public IPlayablePuzzle CreatePlayablePuzzle(Puzzle puzzle)
        {
            return new PlayablePuzzle(columnConstraints: puzzle.ColumnConstraints, rowConstraints: puzzle.RowConstraints);
        }

        public IPlayablePuzzle CreateExtendedPlayablePuzzle(ExtendedPuzzle ePuzzle)
        {
            return new ExtendedPlayablePuzzle(name: ePuzzle.Name.Value, columnConstraints: ePuzzle.Puzzle.Value.ColumnConstraints, rowConstraints: ePuzzle.Puzzle.Value.RowConstraints);
        }
    }
}
